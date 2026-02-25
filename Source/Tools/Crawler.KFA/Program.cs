using System.Text;
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
    var years = new[] { "2025", "2026" };
    var grades = new[] { "초등", "중등", "고등" };
    var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
    var delayMs = 500;

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
            case "--help" or "-h":
                ShowUsage();
                return 0;
        }
    }

    Console.WriteLine($"  Years:  {string.Join(", ", years)}");
    Console.WriteLine($"  Grades: {string.Join(", ", grades)}");
    Console.WriteLine($"  Output: {outputDir}");
    Console.WriteLine($"  Delay:  {delayMs}ms");
    Console.WriteLine();

    using var api = new KfaApiClient();
    var service = new KfaCrawlService(api, delayMs);

    var stats = await service.CrawlAsync(years, grades, outputDir);

    Console.WriteLine();
    Console.WriteLine("========================================");
    Console.WriteLine("Crawling Complete!");
    Console.WriteLine($"  Matches:  {stats.MatchCount}");
    Console.WriteLine($"  Teams:    {stats.TeamCount}");
    Console.WriteLine($"  Players:  {stats.PlayerCount}");
    Console.WriteLine($"  Elapsed:  {stats.Elapsed:mm\\:ss\\.fff}");
    Console.WriteLine($"  Output:   {outputDir}");

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
    Console.WriteLine("Usage: Crawler.KFA [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -y, --year <years>     Target years, comma-separated (default: 2025,2026)");
    Console.WriteLine("  -g, --grade <grades>   Grade filter, comma-separated (default: 초등,중등,고등)");
    Console.WriteLine("  -o, --output <dir>     Output directory (default: ./output)");
    Console.WriteLine("  -d, --delay <ms>       Delay between requests in ms (default: 500)");
    Console.WriteLine("  -h, --help             Show this help");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  Crawler.KFA");
    Console.WriteLine("  Crawler.KFA --year 2025 --grade 고등");
    Console.WriteLine("  Crawler.KFA -y 2025,2026 -g 초등,중등,고등 -d 1000");
}
