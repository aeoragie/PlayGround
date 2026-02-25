using System.Text;
using System.Text.Json;

namespace Crawler.KFA
{
    /// <summary>
    /// KFA 통합경기정보 시스템 API 클라이언트
    /// </summary>
    public class KfaApiClient : IDisposable
    {
        private readonly HttpClient Client;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseUrl = "https://www.joinkfa.com";

        public KfaApiClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.All
            };

            Client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            Client.DefaultRequestHeaders.Add("Accept", "application/json, text/html, */*");
            Client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9");
            Client.DefaultRequestHeaders.Add("Referer", $"{BaseUrl}/service/portal/matchPortal.jsp");
        }

        /// <summary>
        /// 리그/대회 목록 조회
        /// </summary>
        /// <param name="year">연도</param>
        /// <param name="mgcIdx">등급 코드 (1=초등리그, 2=중등리그, 3=고등리그, 51=초등대회, 52=중등대회, 53=고등대회 등)</param>
        /// <param name="page">페이지 번호</param>
        /// <param name="pageSize">페이지당 건수</param>
        /// <param name="style">리그/대회 구분 (LEAGUE2, MATCH)</param>
        public async Task<JsonElement?> GetMatchListAsync(
            string year, string mgcIdx = "", int page = 1, int pageSize = 10, string style = "")
        {
            var parameters = new Dictionary<string, string>
            {
                ["v_CURPAGENUM"] = page.ToString(),
                ["v_ROWCOUNTPERPAGE"] = pageSize.ToString(),
                ["v_YEAR"] = year,
                ["v_STYLE"] = style,
                ["v_MGC_IDX"] = mgcIdx,
                ["v_AREACODE"] = "",
                ["v_SIGUNGU_CODE"] = "",
                ["v_ITEM_CD"] = "",
                ["v_TITLE"] = "",
                ["v_TEAMID"] = "",
                ["v_USER_ID"] = "",
                ["v_ORDERBY"] = ""
            };

            return await PostJsonAsync("/portal/mat/getMatchList.do", parameters);
        }

        /// <summary>
        /// 참가팀 목록 조회
        /// </summary>
        public async Task<JsonElement?> GetApplyTeamListAsync(string matchIdx)
        {
            var parameters = new Dictionary<string, string>
            {
                ["v_MATCH_IDX"] = matchIdx
            };

            return await PostJsonAsync("/portal/mat/getApplyTeamList.do", parameters);
        }

        /// <summary>
        /// 팀 소속 선수 목록 조회
        /// </summary>
        public async Task<JsonElement?> GetApplyPlayerListAsync(string matchIdx, string teamId, string mgcType = "S")
        {
            var parameters = new Dictionary<string, string>
            {
                ["v_MATCH_IDX"] = matchIdx,
                ["v_TEAMID"] = teamId,
                ["v_MGC_TYPE"] = mgcType
            };

            return await PostJsonAsync("/portal/mat/getApplyPlayerList.do", parameters);
        }

        /// <summary>
        /// 경기 일정/결과 조회
        /// </summary>
        public async Task<JsonElement?> GetMatchSingleListAsync(string matchIdx, string yearMonth = "")
        {
            var parameters = new Dictionary<string, string>
            {
                ["v_MATCH_IDX"] = matchIdx,
                ["v_YEAR_MONTH"] = yearMonth,
                ["v_TEAMID"] = ""
            };

            return await PostJsonAsync("/portal/mat/getMatchSingleList.do", parameters);
        }

        /// <summary>
        /// 대회 기본 정보 조회
        /// </summary>
        public async Task<JsonElement?> GetMatchInfoAsync(string matchIdx)
        {
            var parameters = new Dictionary<string, string>
            {
                ["v_MATCH_IDX"] = matchIdx
            };

            return await PostJsonAsync("/portal/mat/getMatchInfo.do", parameters);
        }

        /// <summary>
        /// 초기 데이터 (연도 목록, 필터 코드 등)
        /// </summary>
        public async Task<JsonElement?> GetInitDataAsync()
        {
            return await PostJsonAsync("/portal/mat/getInitData1.do", new Dictionary<string, string>());
        }

        #region HTTP Helper

        private async Task<JsonElement?> PostJsonAsync(string url, Dictionary<string, string> parameters)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(parameters),
                    Encoding.UTF8,
                    "application/json");

                var response = await Client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(json) || json == "{}")
                {
                    return null;
                }

                return JsonSerializer.Deserialize<JsonElement>(json, JsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"[HTTP ERROR] {url}: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"[JSON ERROR] {url}: {ex.Message}");
                return null;
            }
        }

        #endregion

        public void Dispose()
        {
            Client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
