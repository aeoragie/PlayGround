using System.Diagnostics;
using System.Text.Json;
using Crawler.KFA.Models;

namespace Crawler.KFA.Services
{
    /// <summary>
    /// KFA 크롤링 서비스 — 등급별 리그/대회, 팀, 선수 데이터 수집
    /// </summary>
    public class KfaCrawlService
    {
        private readonly KfaApiClient Api;
        private readonly int DelayMs;

        public KfaCrawlService(KfaApiClient api, int delayMs = 500)
        {
            Api = api;
            DelayMs = delayMs;
        }

        /// <summary>
        /// 전체 크롤링 실행
        /// </summary>
        /// <param name="years">크롤링 대상 연도 목록</param>
        /// <param name="grades">등급 필터 (초등, 중등, 고등)</param>
        /// <param name="outputDir">출력 디렉토리</param>
        public async Task<CrawlStats> CrawlAsync(string[] years, string[] grades, string outputDir)
        {
            var stopwatch = Stopwatch.StartNew();
            var stats = new CrawlStats();

            Directory.CreateDirectory(outputDir);

            var gradeCodes = GradeFilter.GetCodes(grades);
            Console.WriteLine($"  Grades: {string.Join(", ", grades)}");
            Console.WriteLine($"  Grade codes: {string.Join(", ", gradeCodes)}");
            Console.WriteLine();

            var allMatches = new List<KfaMatch>();
            var allTeams = new List<KfaTeam>();
            var allPlayers = new List<KfaPlayer>();

            foreach (var year in years)
            {
                Console.WriteLine($"========== Year: {year} ==========");

                // 1단계: 등급별 대회 목록 수집
                Console.WriteLine($"\n[1/3] Fetching match list for {year}...");
                var yearMatches = await CrawlMatchListByGradesAsync(year, gradeCodes);
                Console.WriteLine($"  Found {yearMatches.Count} matches for {year}");
                allMatches.AddRange(yearMatches);

                // 2단계: 각 대회별 참가팀 수집
                Console.WriteLine($"\n[2/3] Fetching teams for {yearMatches.Count} matches...");
                var yearTeams = new List<KfaTeam>();
                foreach (var match in yearMatches)
                {
                    await Task.Delay(DelayMs);
                    var teams = await CrawlTeamListAsync(match.Idx, match.MgcNm);
                    if (teams.Count > 0)
                    {
                        Console.WriteLine($"  [{match.MgcNm}] {match.Title}: {teams.Count} teams");
                        yearTeams.AddRange(teams);
                    }
                }

                // 팀 중복 제거 (같은 팀이 여러 대회에 참가 가능)
                var uniqueYearTeams = yearTeams
                    .GroupBy(t => t.TeamId)
                    .Select(g => g.First())
                    .ToList();
                Console.WriteLine($"  Total unique teams for {year}: {uniqueYearTeams.Count}");
                allTeams.AddRange(yearTeams);

                // 3단계: 각 팀별 선수 목록 수집
                Console.WriteLine($"\n[3/3] Fetching players for {uniqueYearTeams.Count} teams...");
                var teamMatchMap = yearTeams
                    .GroupBy(t => t.TeamId)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var team in uniqueYearTeams)
                {
                    await Task.Delay(DelayMs);
                    var teamInfo = teamMatchMap.GetValueOrDefault(team.TeamId);
                    if (teamInfo == null || string.IsNullOrEmpty(teamInfo.MatchIdx))
                    {
                        continue;
                    }

                    var players = await CrawlPlayerListAsync(
                        teamInfo.MatchIdx, team.TeamId, team.TeamName, team.MgcNm);
                    if (players.Count > 0)
                    {
                        Console.WriteLine($"  [{team.MgcNm}] {team.TeamName}: {players.Count} players");
                        allPlayers.AddRange(players);
                    }
                }

                Console.WriteLine();
            }

            // 최종 중복 제거 및 저장
            var uniqueTeams = allTeams
                .GroupBy(t => t.TeamId)
                .Select(g => g.First())
                .ToList();

            // 선수 중복 제거 (이름 + 팀 + 등번호 기준)
            var uniquePlayers = allPlayers
                .GroupBy(p => $"{p.TeamId}_{p.Name}_{p.EntryNo}")
                .Select(g => g.First())
                .ToList();

            stats.MatchCount = allMatches.Count;
            stats.TeamCount = uniqueTeams.Count;
            stats.PlayerCount = uniquePlayers.Count;

            // JSON 저장
            var yearTag = string.Join("_", years);
            await SaveJsonAsync(Path.Combine(outputDir, $"matches_{yearTag}.json"), allMatches);
            await SaveJsonAsync(Path.Combine(outputDir, $"teams_{yearTag}.json"), uniqueTeams);
            await SaveJsonAsync(Path.Combine(outputDir, $"players_{yearTag}.json"), uniquePlayers);

            stopwatch.Stop();
            stats.Elapsed = stopwatch.Elapsed;

            return stats;
        }

        /// <summary>
        /// 등급별 대회 목록 크롤링
        /// </summary>
        private async Task<List<KfaMatch>> CrawlMatchListByGradesAsync(string year, string[] gradeCodes)
        {
            var allMatches = new List<KfaMatch>();

            foreach (var code in gradeCodes)
            {
                var matches = await CrawlMatchListAsync(year, code);
                Console.WriteLine($"  Grade code [{code}]: {matches.Count} matches");
                allMatches.AddRange(matches);
            }

            // IDX 기준 중복 제거
            return allMatches
                .GroupBy(m => m.Idx)
                .Select(g => g.First())
                .ToList();
        }

        /// <summary>
        /// 단일 등급 코드의 대회 목록 크롤링 (페이징)
        /// </summary>
        private async Task<List<KfaMatch>> CrawlMatchListAsync(string year, string mgcIdx)
        {
            var matches = new List<KfaMatch>();
            var page = 1;
            var pageSize = 10;

            while (true)
            {
                var json = await Api.GetMatchListAsync(year, mgcIdx, page, pageSize);
                if (json == null)
                {
                    break;
                }

                var list = ParseMatchList(json.Value);
                if (list.Count == 0)
                {
                    break;
                }

                matches.AddRange(list);

                // totalCount 확인으로 페이지 끝 판단
                var totalCount = GetTotalCount(json.Value);
                if (matches.Count >= totalCount || list.Count < pageSize)
                {
                    break;
                }

                page++;
                await Task.Delay(DelayMs);
            }

            return matches;
        }

        /// <summary>
        /// 참가팀 목록 크롤링
        /// </summary>
        private async Task<List<KfaTeam>> CrawlTeamListAsync(string matchIdx, string mgcNm)
        {
            var json = await Api.GetApplyTeamListAsync(matchIdx);
            if (json == null)
            {
                return [];
            }

            return ParseTeamList(json.Value, matchIdx, mgcNm);
        }

        /// <summary>
        /// 선수 목록 크롤링
        /// </summary>
        private async Task<List<KfaPlayer>> CrawlPlayerListAsync(
            string matchIdx, string teamId, string teamName, string mgcNm)
        {
            var json = await Api.GetApplyPlayerListAsync(matchIdx, teamId);
            if (json == null)
            {
                return [];
            }

            return ParsePlayerList(json.Value, teamId, teamName, mgcNm);
        }

        #region JSON Parsing

        private static int GetTotalCount(JsonElement json)
        {
            if (json.TryGetProperty("totalCount", out var tc))
            {
                if (tc.ValueKind == JsonValueKind.Number)
                {
                    return tc.GetInt32();
                }
                if (tc.ValueKind == JsonValueKind.String && int.TryParse(tc.GetString(), out var parsed))
                {
                    return parsed;
                }
            }
            return 0;
        }

        private static List<KfaMatch> ParseMatchList(JsonElement json)
        {
            var matches = new List<KfaMatch>();

            if (!TryGetArray(json, "matchList", out var array))
            {
                return matches;
            }

            foreach (var item in array.EnumerateArray())
            {
                matches.Add(new KfaMatch
                {
                    Idx = GetString(item, "IDX"),
                    Title = GetString(item, "TITLE"),
                    MgcNm = GetString(item, "MGC_NM"),
                    StyleNm = GetString(item, "STYLE_NM"),
                    MatchDate = GetString(item, "MA_MCH_DATE"),
                    StartDate = GetString(item, "MA_MCH_STAT_YMD"),
                    EndDate = GetString(item, "MA_MCH_END_YMD"),
                    PlayingArea = GetString(item, "PLAYING_AREA"),
                    SectCnt = GetString(item, "SECT_CNT")
                });
            }

            return matches;
        }

        private static List<KfaTeam> ParseTeamList(JsonElement json, string matchIdx, string mgcNm)
        {
            var teams = new List<KfaTeam>();

            if (!TryGetArray(json, "applyTeamList", out var array))
            {
                return teams;
            }

            foreach (var item in array.EnumerateArray())
            {
                teams.Add(new KfaTeam
                {
                    TeamId = GetString(item, "TEAMID"),
                    TeamName = GetString(item, "TEAMNAME"),
                    Emblem = GetString(item, "EMBLEM"),
                    VirtualTeamYn = GetString(item, "VIRTUAL_TEAM_YN"),
                    MatchIdx = matchIdx,
                    MgcNm = mgcNm
                });
            }

            return teams;
        }

        private static List<KfaPlayer> ParsePlayerList(
            JsonElement json, string teamId, string teamName, string mgcNm)
        {
            var players = new List<KfaPlayer>();

            if (!TryGetArray(json, "applyPlayerList", out var array))
            {
                return players;
            }

            foreach (var item in array.EnumerateArray())
            {
                players.Add(new KfaPlayer
                {
                    Name = GetString(item, "HNAME"),
                    TeamId = teamId,
                    TeamName = GetString(item, "TEAMNAME"),
                    Position = GetString(item, "POSITION"),
                    EntryNo = GetString(item, "ENTRYNO"),
                    PhotoPath = GetString(item, "PHOTO_FILE_PATH"),
                    Photo = GetString(item, "PHOTO"),
                    Birth = GetString(item, "BIRTH"),
                    Height = GetString(item, "HEIGHT"),
                    Weight = GetString(item, "WEIGHT"),
                    IsEnded = GetString(item, "END_YN") == "Y",
                    MgcNm = mgcNm
                });
            }

            return players;
        }

        #endregion

        #region JSON Helpers

        private static bool TryGetArray(JsonElement json, string property, out JsonElement array)
        {
            array = default;

            // 직접 프로퍼티 검색
            if (json.TryGetProperty(property, out array) && array.ValueKind == JsonValueKind.Array)
            {
                return true;
            }

            // 루트가 배열인 경우
            if (json.ValueKind == JsonValueKind.Array)
            {
                array = json;
                return true;
            }

            // 중첩 구조 탐색
            if (json.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in json.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Object &&
                        prop.Value.TryGetProperty(property, out array) &&
                        array.ValueKind == JsonValueKind.Array)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetString(JsonElement item, string property)
        {
            if (item.TryGetProperty(property, out var value))
            {
                return value.ValueKind == JsonValueKind.String
                    ? value.GetString() ?? ""
                    : value.ValueKind == JsonValueKind.Null
                        ? ""
                        : value.ToString();
            }
            return "";
        }

        #endregion

        #region File Output

        private static async Task SaveJsonAsync<T>(string path, T data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var json = JsonSerializer.Serialize(data, options);
            await File.WriteAllTextAsync(path, json, System.Text.Encoding.UTF8);
            Console.WriteLine($"  Saved: {path} ({json.Length:N0} bytes)");
        }

        #endregion
    }
}
