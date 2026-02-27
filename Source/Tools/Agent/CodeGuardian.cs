using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Anthropic.Models.Messages;

namespace Agent
{
    // ─── Code Guardian Agent ──────────────────────────────────────────────────
    //
    // 프로젝트 코드를 주기적으로 스캔하여:
    //   1. 구조 위반 파일 탐지 및 이동
    //   2. 네이밍/타입 중복 정리
    //   3. 결과를 Notion 페이지에 리포트

    internal class CodeGuardianAgent : AgentBase
    {
        private readonly string NotionApiKey;
        private readonly string NotionPageId;

        // Notion REST API 기본 주소
        private const string NotionApiBase = "https://api.notion.com/v1";
        private const string NotionVersion = "2022-06-28";

        public CodeGuardianAgent(
            string apiKey,
            string model,
            int maxTokens,
            string projectRoot,
            string systemPrompt,
            string notionApiKey,
            string notionPageId)
            : base(apiKey, model, maxTokens, projectRoot, systemPrompt)
        {
            NotionApiKey = notionApiKey;
            NotionPageId = notionPageId;
        }

        protected override IReadOnlyList<ToolUnion> GetToolDefinitions()
        {
            return new List<ToolUnion>
            {
                MakeReadFileTool(),
                MakeWriteFileTool(),
                MakeListDirectoryTool(),
                MakeRunCommandTool(),

                // Guardian 전용 도구
                new Tool
                {
                    Name = "search_in_files",
                    Description = "여러 파일에서 패턴(정규식)을 검색합니다. 네임스페이스 위반, 중복 타입 탐지에 사용합니다.",
                    InputSchema = new InputSchema
                    {
                        Properties = new Dictionary<string, JsonElement>
                        {
                            ["directory"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "검색할 디렉터리 경로"
                            }),
                            ["pattern"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "검색할 정규식 패턴 (예: namespace PlayGround\\.Core\\.Shared\\.Domain)"
                            }),
                            ["file_pattern"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "파일 확장자 필터 (예: *.cs, *.sql). 기본값: *.cs"
                            })
                        },
                        Required = new[] { "directory", "pattern" }
                    }
                },

                new Tool
                {
                    Name = "move_file",
                    Description = "파일을 이동하거나 이름을 변경합니다. .cs 파일이면 namespace도 자동으로 업데이트합니다.",
                    InputSchema = new InputSchema
                    {
                        Properties = new Dictionary<string, JsonElement>
                        {
                            ["from_path"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "원본 파일 경로"
                            }),
                            ["to_path"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "대상 파일 경로"
                            }),
                            ["update_namespace"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "boolean",
                                description = ".cs 파일의 namespace를 새 경로에 맞게 업데이트 여부 (기본: true)"
                            })
                        },
                        Required = new[] { "from_path", "to_path" }
                    }
                },

                new Tool
                {
                    Name = "notion_append",
                    Description = "Notion 리포트 페이지에 스캔 결과를 추가합니다. 마크다운 형식의 텍스트를 입력하세요.",
                    InputSchema = new InputSchema
                    {
                        Properties = new Dictionary<string, JsonElement>
                        {
                            ["content"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "추가할 내용. 줄바꿈으로 구분된 항목들을 bullet list로 변환합니다."
                            }),
                            ["title"] = JsonSerializer.SerializeToElement(new
                            {
                                type = "string",
                                description = "섹션 제목 (예: '2026-02-27 스캔 결과')"
                            })
                        },
                        Required = new[] { "content" }
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
                "search_in_files" => ToolSearchInFiles(input),
                "move_file" => ToolMoveFile(input),
                "notion_append" => await ToolNotionAppendAsync(input),
                _ => HandleUnknownTool(toolName)
            };
        }

        // ─── Guardian 전용 도구 구현 ──────────────────────────────────────────

        private string ToolSearchInFiles(IReadOnlyDictionary<string, JsonElement> input)
        {
            string directory = ResolvePath(GetString(input, "directory"));
            string pattern = GetString(input, "pattern");
            string filePattern = input.TryGetValue("file_pattern", out var fp) && fp.ValueKind == JsonValueKind.String
                ? fp.GetString() ?? "*.cs"
                : "*.cs";

            Console.WriteLine($"  검색: '{pattern}' in {directory} ({filePattern})");

            if (!Directory.Exists(directory))
            {
                return $"ERROR: 디렉터리를 찾을 수 없습니다: {directory}";
            }

            Regex regex;

            try
            {
                regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
            catch (RegexParseException ex)
            {
                Debug.Assert(false, $"Invalid regex: {pattern}");
                return $"ERROR: 잘못된 정규식 패턴입니다: {ex.Message}";
            }

            var sb = new StringBuilder();
            int matchCount = 0;
            const int MaxResults = 200;

            foreach (string file in Directory.GetFiles(directory, filePattern, SearchOption.AllDirectories))
            {
                if (matchCount >= MaxResults)
                {
                    sb.AppendLine($"... (최대 {MaxResults}개 결과 초과, 검색 범위를 좁혀주세요)");
                    break;
                }

                string[] lines;

                try
                {
                    lines = File.ReadAllLines(file, Encoding.UTF8);
                }
                catch (IOException)
                {
                    continue;
                }

                string relativePath = Path.GetRelativePath(ProjectRoot, file);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (!regex.IsMatch(lines[i]))
                    {
                        continue;
                    }

                    sb.AppendLine($"{relativePath}:{i + 1}: {lines[i].Trim()}");
                    matchCount++;

                    if (matchCount >= MaxResults)
                    {
                        break;
                    }
                }
            }

            return matchCount > 0
                ? $"총 {matchCount}개 발견:\n{sb}"
                : $"패턴 '{pattern}'과 일치하는 내용 없음";
        }

        private string ToolMoveFile(IReadOnlyDictionary<string, JsonElement> input)
        {
            string fromPath = ResolvePath(GetString(input, "from_path"));
            string toPath = ResolvePath(GetString(input, "to_path"));

            // update_namespace 기본값: true
            bool updateNamespace = !input.TryGetValue("update_namespace", out var un)
                || un.ValueKind != JsonValueKind.False;

            Console.WriteLine($"  이동: {Path.GetRelativePath(ProjectRoot, fromPath)}");
            Console.WriteLine($"     → {Path.GetRelativePath(ProjectRoot, toPath)}");

            if (!File.Exists(fromPath))
            {
                return $"ERROR: 파일을 찾을 수 없습니다: {fromPath}";
            }

            if (File.Exists(toPath))
            {
                return $"ERROR: 대상 파일이 이미 존재합니다: {toPath}";
            }

            string? targetDir = Path.GetDirectoryName(toPath);
            if (!string.IsNullOrEmpty(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // .cs 파일이면 namespace 업데이트
            string? namespaceUpdateInfo = null;

            if (updateNamespace && Path.GetExtension(fromPath).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                namespaceUpdateInfo = UpdateCsNamespace(fromPath, toPath);
            }
            else
            {
                File.Move(fromPath, toPath);
            }

            var sb = new StringBuilder($"이동 완료: {Path.GetRelativePath(ProjectRoot, toPath)}");
            if (namespaceUpdateInfo != null)
            {
                sb.AppendLine($"\n{namespaceUpdateInfo}");
                sb.AppendLine("※ 이 파일을 참조하는 다른 파일의 using 문도 확인하세요.");
                sb.AppendLine($"  search_in_files로 이전 네임스페이스를 검색 후 수정하세요.");
            }

            return sb.ToString();
        }

        // .cs 파일을 새 경로에 맞게 namespace 업데이트 후 이동
        private string UpdateCsNamespace(string fromPath, string toPath)
        {
            string content = File.ReadAllText(fromPath, Encoding.UTF8);

            // 기존 namespace 추출 (block-scoped: namespace Foo.Bar { 또는 file-scoped: namespace Foo.Bar;)
            var nsMatch = Regex.Match(content, @"namespace\s+([\w.]+)\s*[{;]");

            if (!nsMatch.Success)
            {
                // namespace 없는 파일은 그냥 이동
                File.Move(fromPath, toPath);
                return null!;
            }

            string oldNamespace = nsMatch.Groups[1].Value;
            string newNamespace = PathToNamespace(toPath);

            string updatedContent = content.Replace(
                nsMatch.Value,
                nsMatch.Value.Replace(oldNamespace, newNamespace));

            File.WriteAllText(toPath, updatedContent, Encoding.UTF8);
            File.Delete(fromPath);

            return $"namespace 변경: {oldNamespace} → {newNamespace}";
        }

        // 파일 경로에서 PlayGround 네임스페이스 추론
        // 예: Source/Core/Domain/SubDomains/Player/Entities/Player.cs
        //   → PlayGround.Core.Domain.SubDomains.Player.Entities
        private string PathToNamespace(string filePath)
        {
            string relative = Path.GetRelativePath(ProjectRoot, Path.GetDirectoryName(filePath) ?? filePath);

            // Source\ 또는 Source/ 제거
            relative = Regex.Replace(relative, @"^[Ss]ource[/\\]", string.Empty);

            // 경로 구분자를 . 으로 변환
            string ns = relative.Replace('\\', '.').Replace('/', '.');

            return $"PlayGround.{ns}";
        }

        private async Task<string> ToolNotionAppendAsync(IReadOnlyDictionary<string, JsonElement> input)
        {
            if (string.IsNullOrWhiteSpace(NotionApiKey))
            {
                return "ERROR: Notion.ApiKey가 설정되지 않았습니다. appsettings.json을 확인하세요.";
            }

            if (string.IsNullOrWhiteSpace(NotionPageId))
            {
                return "ERROR: Notion.ReportPageId가 설정되지 않았습니다.";
            }

            string content = GetString(input, "content");
            string title = input.TryGetValue("title", out var t) && t.ValueKind == JsonValueKind.String
                ? t.GetString() ?? string.Empty
                : string.Empty;

            Console.WriteLine($"  Notion 업데이트: {NotionPageId[..Math.Min(8, NotionPageId.Length)]}...");

            var children = new List<object>();

            // 제목 블록 추가
            if (!string.IsNullOrWhiteSpace(title))
            {
                children.Add(new
                {
                    type = "heading_2",
                    heading_2 = new
                    {
                        rich_text = new[] { new { type = "text", text = new { content = title } } }
                    }
                });
            }

            // 내용을 줄 단위로 나눠 bullet list 또는 paragraph로 변환
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string trimmed = line.TrimStart('-', '*', ' ').Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    continue;
                }

                bool isBullet = line.TrimStart().StartsWith('-') || line.TrimStart().StartsWith('*');

                if (isBullet)
                {
                    children.Add(new
                    {
                        type = "bulleted_list_item",
                        bulleted_list_item = new
                        {
                            rich_text = new[] { new { type = "text", text = new { content = trimmed } } }
                        }
                    });
                }
                else
                {
                    children.Add(new
                    {
                        type = "paragraph",
                        paragraph = new
                        {
                            rich_text = new[] { new { type = "text", text = new { content = trimmed } } }
                        }
                    });
                }
            }

            if (children.Count == 0)
            {
                return "ERROR: 추가할 내용이 없습니다.";
            }

            var body = new { children };
            string json = JsonSerializer.Serialize(body);

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", NotionApiKey);
            http.DefaultRequestHeaders.Add("Notion-Version", NotionVersion);

            using var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await http.PatchAsync($"{NotionApiBase}/blocks/{NotionPageId}/children", httpContent);

            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Debug.Assert(false, $"Notion API error: {response.StatusCode}");
                return $"ERROR: Notion API 오류 ({response.StatusCode}): {responseBody[..Math.Min(200, responseBody.Length)]}";
            }

            return $"Notion 업데이트 완료. 추가된 블록: {children.Count}개";
        }

        private string HandleUnknownTool(string toolName)
        {
            Debug.Assert(false, $"Unknown tool: {toolName}");
            return $"ERROR: 알 수 없는 도구입니다: {toolName}";
        }
    }
}
