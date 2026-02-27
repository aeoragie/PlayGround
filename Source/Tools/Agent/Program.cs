using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Agent
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string apiKey = config["Anthropic:ApiKey"]
                ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.Error.WriteLine("ANTHROPIC_API_KEY is not configured.");
                return;
            }

            string model = config["Anthropic:Model"] ?? "claude-opus-4-6";
            int maxTokens = config.GetValue("Anthropic:MaxTokens", 16000);
            string dbConnection = config["Database:ConnectionString"] ?? string.Empty;
            string projectRoot = config["Project:RootPath"] ?? Directory.GetCurrentDirectory();
            string notionApiKey = config["Notion:ApiKey"] ?? string.Empty;
            string notionPageId = config["Notion:ReportPageId"] ?? string.Empty;

            // 에이전트 모드 선택
            Console.WriteLine("=== PlayGround Agent ===");
            Console.WriteLine("  [1] Dev Agent     — HTML → DB 설계 → Server/Client 코드 생성");
            Console.WriteLine("  [2] Code Guardian — 코드 스캔 → 구조/네이밍 정리 → Notion 리포트");
            Console.WriteLine();
            Console.Write("모드 선택 (1/2): ");
            string mode = Console.ReadLine()?.Trim() ?? "1";

            AgentBase agent;

            if (mode == "2")
            {
                string promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "CodeGuardianPrompt.md");
                string systemPrompt = File.Exists(promptPath) ? File.ReadAllText(promptPath) : string.Empty;

                agent = new CodeGuardianAgent(apiKey, model, maxTokens, projectRoot, systemPrompt, notionApiKey, notionPageId);
                Console.WriteLine("\n명령 예시:");
                Console.WriteLine("  전체 코드 스캔하고 구조 위반 파일 정리해줘");
                Console.WriteLine("  Source/Core/Shared 폴더에 도메인 코드 있는지 확인해줘");
            }
            else
            {
                string promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "SystemPrompt.md");
                string systemPrompt = File.Exists(promptPath) ? File.ReadAllText(promptPath) : string.Empty;

                agent = new DevAgent(apiKey, model, maxTokens, dbConnection, projectRoot, systemPrompt);
                Console.WriteLine("\n명령 예시:");
                Console.WriteLine("  Others/Template/Player.Dashboard.Preview.html 참고해서 DB 설계, Server, Client 코드 생성해줘");
            }

            Console.WriteLine();
            Console.Write("명령: ");
            string prompt = Console.ReadLine() ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                await agent.RunAsync(prompt);
            }
        }
    }

    // ─── 에이전트 기반 클래스 ─────────────────────────────────────────────────

    internal abstract class AgentBase
    {
        protected readonly AnthropicClient Client;
        protected readonly string Model;
        protected readonly int MaxTokens;
        protected readonly string ProjectRoot;
        protected readonly string SystemPrompt;
        protected readonly List<MessageParam> Messages = new();

        protected static readonly HashSet<string> AllowedCommands = new(StringComparer.OrdinalIgnoreCase)
        {
            "git", "dotnet", "sqlcmd", "npx"
        };

        protected AgentBase(string apiKey, string model, int maxTokens, string projectRoot, string systemPrompt)
        {
            Debug.Assert(!string.IsNullOrEmpty(apiKey), "API key cannot be empty");

            Client = new AnthropicClient { ApiKey = apiKey };
            Model = model;
            MaxTokens = maxTokens;
            ProjectRoot = projectRoot;
            SystemPrompt = systemPrompt;
        }

        protected abstract IReadOnlyList<ToolUnion> GetToolDefinitions();
        protected abstract Task<string> DispatchToolAsync(string toolName, IReadOnlyDictionary<string, JsonElement> input);

        public async Task RunAsync(string userPrompt)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(userPrompt), "User prompt cannot be empty");

            Messages.Add(new MessageParam
            {
                Role = Role.User,
                Content = userPrompt
            });

            while (true)
            {
                var parameters = new MessageCreateParams
                {
                    Model = Model,
                    MaxTokens = MaxTokens,
                    System = string.IsNullOrWhiteSpace(SystemPrompt) ? null : SystemPrompt,
                    Tools = GetToolDefinitions(),
                    Messages = Messages
                };

                Message response;

                try
                {
                    response = await Client.Messages.Create(parameters);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"API call failed: {ex.Message}");
                    return;
                }

                Messages.Add(new MessageParam
                {
                    Role = Role.Assistant,
                    Content = ConvertToContentBlockParams(response.Content)
                });

                if (response.StopReason == StopReason.EndTurn)
                {
                    foreach (var block in response.Content)
                    {
                        if (block.TryPickText(out var textBlock))
                        {
                            Console.WriteLine($"\n{textBlock.Text}");
                            break;
                        }
                    }
                    break;
                }

                if (response.StopReason == StopReason.ToolUse)
                {
                    var toolResults = await ExecuteToolsAsync(response.Content);

                    Messages.Add(new MessageParam
                    {
                        Role = Role.User,
                        Content = toolResults
                    });
                }
                else
                {
                    Debug.Assert(false, $"Unexpected stop reason: {response.StopReason}");
                    break;
                }
            }
        }

        private async Task<List<ContentBlockParam>> ExecuteToolsAsync(IReadOnlyList<ContentBlock> content)
        {
            Debug.Assert(content != null, "Content cannot be null");

            var results = new List<ContentBlockParam>();

            foreach (var block in content)
            {
                if (!block.TryPickToolUse(out var toolUseBlock))
                {
                    continue;
                }

                Console.WriteLine($"\n[도구] {toolUseBlock.Name}");

                string result;

                try
                {
                    result = await DispatchToolAsync(toolUseBlock.Name, toolUseBlock.Input);
                }
                catch (Exception ex)
                {
                    result = $"ERROR: {ex.Message}";
                    Console.Error.WriteLine($"  오류: {ex.Message}");
                }

                results.Add(new ToolResultBlockParam
                {
                    ToolUseID = toolUseBlock.ID,
                    Content = result
                });
            }

            return results;
        }

        // ─── 공통 도구 구현 ───────────────────────────────────────────────────

        protected string ToolReadFile(IReadOnlyDictionary<string, JsonElement> input)
        {
            string path = ResolvePath(GetString(input, "path"));
            Console.WriteLine($"  읽기: {path}");

            if (!File.Exists(path))
            {
                return $"ERROR: 파일을 찾을 수 없습니다: {path}";
            }

            string content = File.ReadAllText(path, Encoding.UTF8);

            const int MaxChars = 50_000;
            if (content.Length > MaxChars)
            {
                return content[..MaxChars] + $"\n\n... (이하 {content.Length - MaxChars}자 생략)";
            }

            return content;
        }

        protected string ToolWriteFile(IReadOnlyDictionary<string, JsonElement> input)
        {
            string path = ResolvePath(GetString(input, "path"));
            string content = GetString(input, "content");

            Console.WriteLine($"  쓰기: {path}");

            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(path, content, Encoding.UTF8);
            return $"저장 완료: {path} ({content.Length}자)";
        }

        protected string ToolListDirectory(IReadOnlyDictionary<string, JsonElement> input)
        {
            string path = ResolvePath(GetString(input, "path"));
            bool recursive = input.TryGetValue("recursive", out var rec) && rec.ValueKind == JsonValueKind.True;

            Console.WriteLine($"  목록: {path}");

            if (!Directory.Exists(path))
            {
                return $"ERROR: 디렉터리를 찾을 수 없습니다: {path}";
            }

            var sb = new StringBuilder();
            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (string dir in Directory.GetDirectories(path, "*", option))
            {
                sb.AppendLine($"[DIR]  {Path.GetRelativePath(path, dir)}");
            }

            foreach (string file in Directory.GetFiles(path, "*", option))
            {
                var info = new FileInfo(file);
                sb.AppendLine($"[FILE] {Path.GetRelativePath(path, file)} ({info.Length / 1024}KB)");
            }

            return sb.Length > 0 ? sb.ToString() : "(비어있음)";
        }

        protected async Task<string> ToolRunCommandAsync(IReadOnlyDictionary<string, JsonElement> input)
        {
            string command = GetString(input, "command");
            string workingDir = input.TryGetValue("working_dir", out var wd) && wd.ValueKind == JsonValueKind.String
                ? ResolvePath(wd.GetString() ?? ProjectRoot)
                : ProjectRoot;

            string commandName = command.Split(' ')[0].Trim();
            if (!AllowedCommands.Contains(commandName))
            {
                Debug.Assert(false, $"Command not allowed: {commandName}");
                return $"ERROR: 허용되지 않은 명령입니다: {commandName}. 허용 목록: {string.Join(", ", AllowedCommands)}";
            }

            Console.WriteLine($"  실행: {command}");

            var psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)
                ?? throw new InvalidOperationException("프로세스를 시작할 수 없습니다.");

            string stdout = await process.StandardOutput.ReadToEndAsync();
            string stderr = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = stdout;
            if (!string.IsNullOrWhiteSpace(stderr))
            {
                output += $"\n[STDERR]\n{stderr}";
            }

            return string.IsNullOrWhiteSpace(output) ? $"완료 (종료코드: {process.ExitCode})" : output;
        }

        // ─── 헬퍼 ─────────────────────────────────────────────────────────────

        protected string ResolvePath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            return Path.GetFullPath(Path.Combine(ProjectRoot, path));
        }

        protected static string GetString(IReadOnlyDictionary<string, JsonElement> input, string key)
        {
            return input.TryGetValue(key, out var val) && val.ValueKind == JsonValueKind.String
                ? val.GetString() ?? string.Empty
                : string.Empty;
        }

        protected static List<ContentBlockParam> ConvertToContentBlockParams(IReadOnlyList<ContentBlock> blocks)
        {
            var result = new List<ContentBlockParam>();

            foreach (var block in blocks)
            {
                if (block.TryPickText(out var textBlock))
                {
                    result.Add(new TextBlockParam(textBlock.Text));
                }
                else if (block.TryPickToolUse(out var toolUseBlock))
                {
                    result.Add(new ToolUseBlockParam
                    {
                        ID = toolUseBlock.ID,
                        Name = toolUseBlock.Name,
                        Input = toolUseBlock.Input
                    });
                }
            }

            return result;
        }

        // ─── 공통 도구 정의 (서브클래스에서 조합) ────────────────────────────

        protected static ToolUnion MakeReadFileTool() => new Tool
        {
            Name = "read_file",
            Description = "파일 내용을 읽습니다.",
            InputSchema = new InputSchema
            {
                Properties = new Dictionary<string, JsonElement>
                {
                    ["path"] = JsonSerializer.SerializeToElement(new { type = "string", description = "파일 경로 (절대 또는 프로젝트 루트 기준 상대)" })
                },
                Required = new[] { "path" }
            }
        };

        protected static ToolUnion MakeWriteFileTool() => new Tool
        {
            Name = "write_file",
            Description = "파일을 생성하거나 덮어씁니다.",
            InputSchema = new InputSchema
            {
                Properties = new Dictionary<string, JsonElement>
                {
                    ["path"] = JsonSerializer.SerializeToElement(new { type = "string", description = "파일 경로" }),
                    ["content"] = JsonSerializer.SerializeToElement(new { type = "string", description = "파일 내용" })
                },
                Required = new[] { "path", "content" }
            }
        };

        protected static ToolUnion MakeListDirectoryTool() => new Tool
        {
            Name = "list_directory",
            Description = "디렉터리 파일/폴더 목록을 가져옵니다.",
            InputSchema = new InputSchema
            {
                Properties = new Dictionary<string, JsonElement>
                {
                    ["path"] = JsonSerializer.SerializeToElement(new { type = "string", description = "디렉터리 경로" }),
                    ["recursive"] = JsonSerializer.SerializeToElement(new { type = "boolean", description = "재귀 조회 여부 (기본: false)" })
                },
                Required = new[] { "path" }
            }
        };

        protected static ToolUnion MakeRunCommandTool() => new Tool
        {
            Name = "run_command",
            Description = "git, dotnet, npx 명령을 실행합니다.",
            InputSchema = new InputSchema
            {
                Properties = new Dictionary<string, JsonElement>
                {
                    ["command"] = JsonSerializer.SerializeToElement(new { type = "string", description = "실행할 명령 (예: git add .)" }),
                    ["working_dir"] = JsonSerializer.SerializeToElement(new { type = "string", description = "실행 디렉터리 (기본: 프로젝트 루트)" })
                },
                Required = new[] { "command" }
            }
        };
    }

    // ─── Dev Agent ────────────────────────────────────────────────────────────

    internal class DevAgent : AgentBase
    {
        private readonly string DbConnectionString;

        public DevAgent(
            string apiKey,
            string model,
            int maxTokens,
            string dbConnectionString,
            string projectRoot,
            string systemPrompt)
            : base(apiKey, model, maxTokens, projectRoot, systemPrompt)
        {
            DbConnectionString = dbConnectionString;
        }

        protected override IReadOnlyList<ToolUnion> GetToolDefinitions()
        {
            return new List<ToolUnion>
            {
                MakeReadFileTool(),
                MakeWriteFileTool(),
                MakeListDirectoryTool(),
                MakeRunCommandTool(),
                new Tool
                {
                    Name = "execute_sql",
                    Description = "MSSQL에 SQL을 실행합니다. DDL(CREATE TABLE, SP)과 SELECT 모두 지원.",
                    InputSchema = new InputSchema
                    {
                        Properties = new Dictionary<string, JsonElement>
                        {
                            ["sql"] = JsonSerializer.SerializeToElement(new { type = "string", description = "실행할 SQL 문" })
                        },
                        Required = new[] { "sql" }
                    }
                }
            };
        }

        protected override async Task<string> DispatchToolAsync(string toolName, IReadOnlyDictionary<string, JsonElement> input)
        {
            return toolName switch
            {
                "read_file" => ToolReadFile(input),
                "write_file" => ToolWriteFile(input),
                "list_directory" => ToolListDirectory(input),
                "run_command" => await ToolRunCommandAsync(input),
                "execute_sql" => await ToolExecuteSqlAsync(input),
                _ => HandleUnknownTool(toolName)
            };
        }

        private async Task<string> ToolExecuteSqlAsync(IReadOnlyDictionary<string, JsonElement> input)
        {
            if (string.IsNullOrWhiteSpace(DbConnectionString))
            {
                return "ERROR: Database.ConnectionString이 설정되지 않았습니다.";
            }

            string sql = GetString(input, "sql");
            Console.WriteLine($"  SQL: {sql[..Math.Min(80, sql.Length)]}...");

            using var conn = new SqlConnection(DbConnectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 60 };

            bool isDdl = sql.TrimStart().StartsWith("CREATE", StringComparison.OrdinalIgnoreCase)
                || sql.TrimStart().StartsWith("ALTER", StringComparison.OrdinalIgnoreCase)
                || sql.TrimStart().StartsWith("DROP", StringComparison.OrdinalIgnoreCase)
                || sql.TrimStart().StartsWith("INSERT", StringComparison.OrdinalIgnoreCase)
                || sql.TrimStart().StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase)
                || sql.TrimStart().StartsWith("DELETE", StringComparison.OrdinalIgnoreCase);

            if (isDdl)
            {
                int rows = await cmd.ExecuteNonQueryAsync();
                return $"실행 완료. 영향받은 행: {rows}";
            }

            using var reader = await cmd.ExecuteReaderAsync();
            var sb = new StringBuilder();
            int rowCount = 0;

            while (await reader.ReadAsync() && rowCount < 100)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    sb.Append($"{reader.GetName(i)}: {reader[i]}  ");
                }
                sb.AppendLine();
                rowCount++;
            }

            return rowCount > 0 ? sb.ToString() : "(결과 없음)";
        }

        private string HandleUnknownTool(string toolName)
        {
            Debug.Assert(false, $"Unknown tool: {toolName}");
            return $"ERROR: 알 수 없는 도구입니다: {toolName}";
        }
    }
}
