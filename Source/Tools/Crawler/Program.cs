using System.Text;
using System.Text.Json;
using Crawler.KFA;
using Crawler.KFA.Services;

Console.Title = "KFA Crawler";
Console.OutputEncoding = Encoding.UTF8;

try
{
    Console.WriteLine("KFA Crawler - 통합경기정보 시스템 크롤러");
    Console.WriteLine("========================================");
    Console.WriteLine();

    // 기본값
    //var years = new[] { "2025", "2026" };
    var years = new[] { "2026" };
    var grades = new[] { "초등", "중등", "고등" };
    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
    var delayMs = 500;
    int? limit = null;
    string? testMatchIdx = null;
    string? nexacroUser = null;
    string? nexacroSecret = null;
    string? jsessionId = null;
    var parallel = 4;

    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--year" or "-y" when i + 1 < args.Length:
                years = args[++i].Split(',');
                break;
            case "--grade" or "-g" when i + 1 < args.Length:
                grades = args[++i].Split(',');
                break;
            case "--output" or "-o" when i + 1 < args.Length:
                outputDir = args[++i];
                break;
            case "--delay" or "-d" when i + 1 < args.Length:
                delayMs = int.Parse(args[++i]);
                break;
            case "--limit" or "-l" when i + 1 < args.Length:
                limit = int.Parse(args[++i]);
                break;
            case "--test" or "-t" when i + 1 < args.Length:
                testMatchIdx = args[++i];
                break;
            case "--user" or "-u" when i + 1 < args.Length:
                nexacroUser = args[++i];
                break;
            case "--secret" or "-s" when i + 1 < args.Length:
                nexacroSecret = args[++i];
                break;
            case "--jsessionid" or "-j" when i + 1 < args.Length:
                jsessionId = args[++i];
                break;
            case "--parallel" or "-p" when i + 1 < args.Length:
                parallel = int.Parse(args[++i]);
                break;
            case "--help" or "-h":
                ShowUsage();
                return 0;
        }
    }

    using var api = new KfaApiClient();
    api.NexacroUserId = nexacroUser;
    api.NexacroSecret = nexacroSecret;
    api.SetAuthCookies(jsessionId);

    // --test 모드: 특정 대회의 경기 결과 + 상세 API 테스트
    if (testMatchIdx != null)
    {
        Console.WriteLine($"[TEST MODE] matchIdx={testMatchIdx}");
        Console.WriteLine($"  Auth: {(api.HasNexacroAuth ? "enabled" : "disabled")}");
        Directory.CreateDirectory(outputDir);

        // 경기 결과 조회 (2026-02 고정)
        var yearMonth = "2026-02";
        Console.WriteLine($"\n--- getMatchSingleList (v_YEAR_MONTH={yearMonth}) ---");
        var json = await api.GetMatchSingleListAsync(testMatchIdx, yearMonth);
        if (json == null)
        {
            Console.WriteLine("  Result: null");
            return 0;
        }

        var raw = JsonSerializer.Serialize(json.Value, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine($"  Result: {raw.Length:N0} bytes");

        // singleList에서 첫 번째 완료 경기 찾기
        if (json.Value.TryGetProperty("singleList", out var list) && list.ValueKind == JsonValueKind.Array)
        {
            string? singleIdx = null;
            string? homeTeam = null;
            string? awayTeam = null;

            foreach (var item in list.EnumerateArray())
            {
                var score = item.TryGetProperty("TH_SCORE_FINAL", out var s) ? s.GetString() : "";
                if (!string.IsNullOrEmpty(score))
                {
                    singleIdx = item.TryGetProperty("IDX", out var idx) ? idx.GetString() : null;
                    homeTeam = item.TryGetProperty("TEAM_HOME", out var h) ? h.GetString() : "";
                    awayTeam = item.TryGetProperty("TEAM_AWAY", out var a) ? a.GetString() : "";
                    break;
                }
            }

            if (singleIdx != null && api.HasNexacroAuth)
            {
                Console.WriteLine($"\n--- SEARCH00.do (matchDetail) ---");
                Console.WriteLine($"  Target: {homeTeam} vs {awayTeam} (singleIdx={singleIdx})");

                var detail = await api.GetMatchDetailAsync(testMatchIdx, singleIdx);
                if (detail == null || detail.Count == 0)
                {
                    Console.WriteLine("  Result: null or empty");
                }
                else
                {
                    Console.WriteLine($"  Datasets: {string.Join(", ", detail.Keys)}");
                    foreach (var ds in detail)
                    {
                        Console.WriteLine($"    {ds.Key}: {ds.Value.Count} rows");
                    }

                    var detailJson = JsonSerializer.Serialize(detail, new JsonSerializerOptions { WriteIndented = true });
                    var path = Path.Combine(outputDir, $"debug_matchDetail_{singleIdx[..8]}.json");
                    await File.WriteAllTextAsync(path, detailJson, Encoding.UTF8);
                    Console.WriteLine($"  Saved: {path}");
                }
            }
            else if (singleIdx == null)
            {
                Console.WriteLine("\n  No finished games found in results.");
            }
            else
            {
                Console.WriteLine("\n  Auth disabled — skip match detail test. Use --user/--secret.");
            }
        }

        return 0;
    }

    Console.WriteLine($"  Years:  {string.Join(", ", years)}");
    Console.WriteLine($"  Grades: {string.Join(", ", grades)}");
    Console.WriteLine($"  Output: {outputDir}");
    Console.WriteLine($"  Delay:  {delayMs}ms");
    Console.WriteLine($"  Limit:  {(limit.HasValue ? $"{limit} matches" : "none")}");
    Console.WriteLine($"  Parallel: {parallel}");
    Console.WriteLine($"  Detail: {(nexacroUser != null ? "enabled" : "disabled (use --user/--secret)")}");
    Console.WriteLine();

    var service = new KfaCrawlService(api, delayMs, parallel);

    var stats = await service.CrawlAsync(years, grades, outputDir, limit);

    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine("Crawling Complete!");
    Console.WriteLine($"  Matches:       {stats.MatchCount}");
    Console.WriteLine($"  GameResults:   {stats.MatchResultCount}");
    Console.WriteLine($"  GameDetails:   {stats.MatchDetailCount}");
    Console.WriteLine($"  Teams:         {stats.TeamCount}");
    Console.WriteLine($"  Players:       {stats.PlayerCount}");
    Console.WriteLine($"  Elapsed:       {stats.Elapsed:mm\\:ss\\.fff}");
    Console.WriteLine($"  Output:        {outputDir}");

    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"\n[FATAL] {ex.Message}");
    Console.Error.WriteLine(ex.StackTrace);
    return 1;
}

static void ShowUsage()
{
    Console.WriteLine("Usage: Crawler [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -y, --year <years>     Target years, comma-separated (Default: 2026)");
    Console.WriteLine("  -g, --grade <grades>   Grade filter, comma-separated (Default: 초등,중등,고등)");
    Console.WriteLine("  -o, --output <dir>     Output directory (default: ./Output)");
    Console.WriteLine("  -d, --delay <ms>       Delay between requests in ms (Default: 500)");
    Console.WriteLine("  -l, --limit <n>        Limit number of matches to crawl (for debugging)");
    Console.WriteLine("  -u, --user <id>        Nexacro user ID (for match detail crawling)");
    Console.WriteLine("  -s, --secret <key>     Nexacro secret (from browser Cookie: state=secret%3D...)");
    Console.WriteLine("  -j, --jsessionid <id>  JSESSIONID (from browser Cookie)");
    Console.WriteLine("  -p, --parallel <n>     Max concurrent requests (Default: 4)");
    Console.WriteLine("  -h, --help             Show this help");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  Crawler");
    Console.WriteLine("  Crawler --year 2025 --grade 고등");
    Console.WriteLine("  Crawler -y 2025,2026 -g 초등,중등,고등 -d 1000");
    Console.WriteLine("  Crawler --user myid --secret abc123 --jsessionid ABCDEF  (with match detail)");
}
