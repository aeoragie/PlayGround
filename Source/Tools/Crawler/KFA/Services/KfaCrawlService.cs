using System.Collections.Concurrent;
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
        private readonly SemaphoreSlim Throttle;

        public KfaCrawlService(KfaApiClient api, int delayMs = 500, int maxConcurrency = 4)
        {
            Api = api;
            DelayMs = delayMs;
            Throttle = new SemaphoreSlim(maxConcurrency);
        }

        /// <summary>
        /// 전체 크롤링 실행
        /// </summary>
        /// <param name="years">크롤링 대상 연도 목록</param>
        /// <param name="grades">등급 필터 (초등, 중등, 고등)</param>
        /// <param name="outputDir">출력 디렉토리</param>
        public async Task<CrawlStats> CrawlAsync(string[] years, string[] grades, string outputDir, int? limit = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var stats = new CrawlStats();

            Directory.CreateDirectory(outputDir);

            var gradeCodes = GradeFilter.GetCodes(grades);
            Console.WriteLine($"  Grades: {string.Join(", ", grades)}");
            Console.WriteLine($"  Grade Codes: {string.Join(", ", gradeCodes)}");
            Console.WriteLine();

            var allMatches = new List<KfaMatch>();
            var allMatchResults = new List<KfaMatchResult>();
            var allMatchDetails = new List<KfaMatchDetail>();
            var allTeams = new List<KfaTeam>();
            var allPlayers = new List<KfaPlayer>();

            var hasAuth = Api.HasNexacroAuth;
            var totalSteps = hasAuth ? 5 : 4;

            foreach (var year in years)
            {
                Console.WriteLine($"========== Year: {year} ==========");

                var step = 0;

                // 1단계: 등급별 대회 목록 수집
                step++;
                Console.WriteLine($"\n[{step}/{totalSteps}] Fetching match list for {year}...");
                var yearMatches = await CrawlMatchListByGradesAsync(year, gradeCodes);
                Console.WriteLine($"  Found {yearMatches.Count} matches for {year}");

                if (limit.HasValue && yearMatches.Count > limit.Value)
                {
                    yearMatches = yearMatches.Take(limit.Value).ToList();
                    Console.WriteLine($"  Limited to {yearMatches.Count} matches (--limit {limit.Value})");
                }

                allMatches.AddRange(yearMatches);

                // 2단계: 각 대회별 경기 결과 수집 (병렬)
                step++;
                Console.WriteLine($"\n[{step}/{totalSteps}] Fetching match results for {yearMatches.Count} matches...");
                var yearMatchResults = await RunParallelAsync(yearMatches,
                    m => CrawlMatchResultsAsync(m.Idx, m.MgcNm, m.StartDate, m.EndDate),
                    (m, count) => $"[{m.MgcNm}] {m.Title}: {count} games");
                allMatchResults.AddRange(yearMatchResults);

                // 3단계: 경기 상세 수집 (인증 시에만, 병렬)
                if (hasAuth)
                {
                    step++;
                    var finishedResults = yearMatchResults
                        .Where(r => !string.IsNullOrEmpty(r.SingleIdx) && !string.IsNullOrEmpty(r.HomeScore))
                        .ToList();
                    Console.WriteLine($"\n[{step}/{totalSteps}] Fetching match details for {finishedResults.Count} finished games...");

                    var yearDetails = await RunParallelSingleAsync(finishedResults,
                        r => CrawlMatchDetailAsync(r.MatchIdx, r.SingleIdx, r.MgcNm),
                        (r, d) => $"[{r.MgcNm}] {r.HomeTeam} vs {r.AwayTeam}: {d.Events.Count} events, {d.HomeStarters.Count + d.AwayStarters.Count} starters");
                    allMatchDetails.AddRange(yearDetails);
                }

                // 4단계: 참가팀 수집 (병렬)
                step++;
                Console.WriteLine($"\n[{step}/{totalSteps}] Fetching teams for {yearMatches.Count} matches...");
                var yearTeams = await RunParallelAsync(yearMatches,
                    m => CrawlTeamListAsync(m.Idx, m.MgcNm),
                    (m, count) => $"[{m.MgcNm}] {m.Title}: {count} teams");

                // 팀 중복 제거 (같은 팀이 여러 대회에 참가 가능)
                var uniqueYearTeams = yearTeams
                    .GroupBy(t => t.TeamId)
                    .Select(g => g.First())
                    .ToList();
                Console.WriteLine($"  Total unique teams for {year}: {uniqueYearTeams.Count}");
                allTeams.AddRange(yearTeams);

                // 5단계: 선수 목록 수집 (병렬)
                step++;
                Console.WriteLine($"\n[{step}/{totalSteps}] Fetching players for {uniqueYearTeams.Count} teams...");
                var teamMatchMap = yearTeams
                    .GroupBy(t => t.TeamId)
                    .ToDictionary(g => g.Key, g => g.First());

                var teamsWithMatch = uniqueYearTeams
                    .Where(t => teamMatchMap.ContainsKey(t.TeamId) &&
                                !string.IsNullOrEmpty(teamMatchMap[t.TeamId].MatchIdx))
                    .ToList();

                var yearPlayers = await RunParallelAsync(teamsWithMatch,
                    t => CrawlPlayerListAsync(
                        teamMatchMap[t.TeamId].MatchIdx, t.TeamId, t.TeamName, t.MgcNm),
                    (t, count) => $"[{t.MgcNm}] {t.TeamName}: {count} players");
                allPlayers.AddRange(yearPlayers);

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
            stats.MatchResultCount = allMatchResults.Count;
            stats.MatchDetailCount = allMatchDetails.Count;
            stats.TeamCount = uniqueTeams.Count;
            stats.PlayerCount = uniquePlayers.Count;

            // JSON 저장
            var yearTag = string.Join("_", years);
            await SaveJsonAsync(Path.Combine(outputDir, $"Matches_{yearTag}.json"), allMatches);
            await SaveJsonAsync(Path.Combine(outputDir, $"Match_Results_{yearTag}.json"), allMatchResults);
            if (allMatchDetails.Count > 0)
            {
                await SaveJsonAsync(Path.Combine(outputDir, $"Match_Details_{yearTag}.json"), allMatchDetails);
            }
            await SaveJsonAsync(Path.Combine(outputDir, $"Teams_{yearTag}.json"), uniqueTeams);
            await SaveJsonAsync(Path.Combine(outputDir, $"Players_{yearTag}.json"), uniquePlayers);

            stopwatch.Stop();
            stats.Elapsed = stopwatch.Elapsed;

            return stats;
        }

        /// <summary>
        /// 등급별 대회 목록 크롤링 (병렬)
        /// </summary>
        private async Task<List<KfaMatch>> CrawlMatchListByGradesAsync(string year, string[] gradeCodes)
        {
            var allMatches = await RunParallelAsync(
                gradeCodes,
                code => CrawlMatchListAsync(year, code),
                (code, count) => $"Grade code [{code}]: {count} matches");

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
        /// 경기 상세 크롤링 (Nexacro)
        /// </summary>
        private async Task<KfaMatchDetail?> CrawlMatchDetailAsync(string matchIdx, string singleIdx, string mgcNm)
        {
            var datasets = await Api.GetMatchDetailAsync(matchIdx, singleIdx);
            if (datasets == null || datasets.Count == 0)
            {
                return null;
            }

            return ParseMatchDetail(datasets, matchIdx, singleIdx, mgcNm);
        }

        /// <summary>
        /// 경기 결과 크롤링 — 대회 기간의 각 월별로 API 호출
        /// </summary>
        private async Task<List<KfaMatchResult>> CrawlMatchResultsAsync(
            string matchIdx, string mgcNm, string startDate, string endDate)
        {
            var months = GetMonthRange(startDate, endDate);
            if (months.Count == 0)
            {
                return [];
            }

            var allResults = new List<KfaMatchResult>();

            foreach (var month in months)
            {
                var json = await Api.GetMatchSingleListAsync(matchIdx, month);
                if (json == null)
                {
                    continue;
                }

                var results = ParseMatchResultList(json.Value, matchIdx, mgcNm);
                allResults.AddRange(results);

                if (months.Count > 1)
                {
                    await Task.Delay(DelayMs);
                }
            }

            // SingleIdx 기준 중복 제거 (월이 겹칠 수 있음)
            return allResults
                .GroupBy(r => r.SingleIdx)
                .Select(g => g.First())
                .ToList();
        }

        /// <summary>
        /// 시작일~종료일 범위의 YYYY-MM 목록 생성
        /// </summary>
        private static List<string> GetMonthRange(string startDate, string endDate)
        {
            if (!DateTime.TryParse(startDate, out var start))
            {
                return [];
            }

            if (!DateTime.TryParse(endDate, out var end))
            {
                end = start;
            }

            var months = new List<string>();
            var current = new DateTime(start.Year, start.Month, 1);
            var last = new DateTime(end.Year, end.Month, 1);

            while (current <= last)
            {
                months.Add(current.ToString("yyyy-MM"));
                current = current.AddMonths(1);
            }

            return months;
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

        #region Parallel Execution

        /// <summary>
        /// 리스트 반환 작업의 병렬 실행 (SemaphoreSlim 스로틀링)
        /// </summary>
        private async Task<List<TResult>> RunParallelAsync<TItem, TResult>(
            IReadOnlyList<TItem> items,
            Func<TItem, Task<List<TResult>>> action,
            Func<TItem, int, string>? logFormat = null)
        {
            var results = new ConcurrentBag<(int Index, List<TResult> Items)>();

            var tasks = items.Select(async (item, index) =>
            {
                await Throttle.WaitAsync();
                try
                {
                    await Task.Delay(DelayMs);
                    var result = await action(item);
                    if (result.Count > 0)
                    {
                        results.Add((index, result));
                        if (logFormat != null)
                        {
                            Console.WriteLine($"  {logFormat(item, result.Count)}");
                        }
                    }
                }
                finally
                {
                    Throttle.Release();
                }
            });

            await Task.WhenAll(tasks);

            // 원래 순서 유지
            return results.OrderBy(r => r.Index).SelectMany(r => r.Items).ToList();
        }

        /// <summary>
        /// 단일 결과 반환 작업의 병렬 실행
        /// </summary>
        private async Task<List<TResult>> RunParallelSingleAsync<TItem, TResult>(
            IReadOnlyList<TItem> items,
            Func<TItem, Task<TResult?>> action,
            Func<TItem, TResult, string>? logFormat = null) where TResult : class
        {
            var results = new ConcurrentBag<(int Index, TResult Item)>();

            var tasks = items.Select(async (item, index) =>
            {
                await Throttle.WaitAsync();
                try
                {
                    await Task.Delay(DelayMs);
                    var result = await action(item);
                    if (result != null)
                    {
                        results.Add((index, result));
                        if (logFormat != null)
                        {
                            Console.WriteLine($"  {logFormat(item, result)}");
                        }
                    }
                }
                finally
                {
                    Throttle.Release();
                }
            });

            await Task.WhenAll(tasks);

            return results.OrderBy(r => r.Index).Select(r => r.Item).ToList();
        }

        #endregion

        #region JSON Parsing (Portal API)

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

        private static List<KfaMatchResult> ParseMatchResultList(JsonElement json, string matchIdx, string mgcNm)
        {
            var results = new List<KfaMatchResult>();

            if (!TryGetArray(json, "singleList", out var array))
            {
                return results;
            }

            foreach (var item in array.EnumerateArray())
            {
                results.Add(new KfaMatchResult
                {
                    SingleIdx = GetString(item, "IDX"),
                    MatchIdx = matchIdx,
                    MatchNumber = GetString(item, "MATCH_NUMBER"),
                    MatchGroup = GetString(item, "MATCH_GROUP"),
                    Time = GetString(item, "TIME"),
                    MatchArea = GetString(item, "MATCH_AREA"),
                    MatchDate = GetString(item, "MATCH_CHECK_TIME2"),
                    HomeTeam = GetString(item, "TEAM_HOME"),
                    AwayTeam = GetString(item, "TEAM_AWAY"),
                    HomeScore = GetString(item, "TH_SCORE_FINAL"),
                    AwayScore = GetString(item, "TA_SCORE_FINAL"),
                    HomePkScore = GetString(item, "TH_SCORE_PK"),
                    AwayPkScore = GetString(item, "TA_SCORE_PK"),
                    ScoreText = GetString(item, "SCORE_TXT"),
                    MgcNm = mgcNm
                });
            }

            return results;
        }

        #region Nexacro Parsing

        private static KfaMatchDetail? ParseMatchDetail(
            Dictionary<string, List<Dictionary<string, string>>> datasets,
            string matchIdx, string singleIdx, string mgcNm)
        {
            var common = NexacroHelper.GetFirstRow(datasets, "dsCommonInfo");
            if (common == null)
            {
                return null;
            }

            var detail = new KfaMatchDetail
            {
                SingleIdx = singleIdx,
                MatchIdx = matchIdx,
                MatchNumber = NexacroHelper.Get(common, "MATCH_NUMBER"),
                MatchDate = NexacroHelper.Get(common, "MATCH_CHECK_TIME"),
                MatchArea = NexacroHelper.Get(common, "MATCH_AREA"),
                Title = NexacroHelper.Get(common, "TITLE"),
                HomeTeam = NexacroHelper.Get(common, "TEAM_HOME"),
                AwayTeam = NexacroHelper.Get(common, "TEAM_AWAY"),
                HomeScoreFinal = NexacroHelper.Get(common, "TH_SCORE_FINAL"),
                AwayScoreFinal = NexacroHelper.Get(common, "TA_SCORE_FINAL"),
                HomeScoreFirstHalf = NexacroHelper.Get(common, "TH_SCORE_FH"),
                AwayScoreFirstHalf = NexacroHelper.Get(common, "TA_SCORE_FH"),
                HomeScoreSecondHalf = NexacroHelper.Get(common, "TH_SCORE_SH"),
                AwayScoreSecondHalf = NexacroHelper.Get(common, "TA_SCORE_SH"),
                HomeScorePk = NexacroHelper.Get(common, "TH_SCORE_PK"),
                AwayScorePk = NexacroHelper.Get(common, "TA_SCORE_PK"),
                RefereeMain = NexacroHelper.Get(common, "REFEREE_MAIN"),
                Weather = NexacroHelper.Get(common, "WEATHER"),
                Viewers = NexacroHelper.Get(common, "VIEWERS"),
                PlayingTime = NexacroHelper.Get(common, "PLAYING_TIME"),
                TotalTime = NexacroHelper.Get(common, "TOTALTIME"),
                CoachHome = NexacroHelper.Get(common, "COACH_HOME"),
                CoachAway = NexacroHelper.Get(common, "COACH_AWAY"),
                HomeYellowCount = NexacroHelper.Get(common, "TH_YELLOW_CNT"),
                AwayYellowCount = NexacroHelper.Get(common, "TA_YELLOW_CNT"),
                HomeRedCount = NexacroHelper.Get(common, "TH_RED_CNT"),
                AwayRedCount = NexacroHelper.Get(common, "TA_RED_CNT"),
                MgcNm = mgcNm
            };

            // 타임라인 이벤트
            detail.Events = ParseEvents(NexacroHelper.GetRows(datasets, "dsTimelineInfo"));

            // 라인업
            detail.HomeStarters = ParseLineup(NexacroHelper.GetRows(datasets, "dsHomePlayerInfo1"));
            detail.HomeSubstitutes = ParseLineup(NexacroHelper.GetRows(datasets, "dsHomePlayerInfo2"));
            detail.AwayStarters = ParseLineup(NexacroHelper.GetRows(datasets, "dsAwayPlayerInfo1"));
            detail.AwaySubstitutes = ParseLineup(NexacroHelper.GetRows(datasets, "dsAwayPlayerInfo2"));

            return detail;
        }

        private static List<KfaMatchEvent> ParseEvents(List<Dictionary<string, string>> rows)
        {
            return rows.Select(r => new KfaMatchEvent
            {
                PlayerName = NexacroHelper.Get(r, "HNAME"),
                EventType = NexacroHelper.Get(r, "FLAG_NM"),
                EventCode = NexacroHelper.Get(r, "FLAG"),
                Time = NexacroHelper.Get(r, "TIME"),
                EntryNo = NexacroHelper.Get(r, "ENTRYNO"),
                Side = NexacroHelper.Get(r, "GUBUN"),
                IsPk = NexacroHelper.Get(r, "PKYN")
            }).ToList();
        }

        private static List<KfaLineupPlayer> ParseLineup(List<Dictionary<string, string>> rows)
        {
            return rows.Select(r => new KfaLineupPlayer
            {
                Name = NexacroHelper.Get(r, "HNAME"),
                Position = NexacroHelper.Get(r, "POSITION"),
                EntryNo = NexacroHelper.Get(r, "ENTRYNO"),
                PlayTime = NexacroHelper.Get(r, "TIME"),
                Status = NexacroHelper.Get(r, "STATUS"),
                IsCaptain = NexacroHelper.Get(r, "C_CHECK"),
                GoalTime = NexacroHelper.Get(r, "GOAL_TIME"),
                YellowTime = NexacroHelper.Get(r, "YELLOW_TIME"),
                RedTime = NexacroHelper.Get(r, "RED_TIME"),
                AssistTime = NexacroHelper.Get(r, "HELP_TIME")
            }).ToList();
        }

        #endregion

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
