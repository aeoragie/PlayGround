using System.Net;
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
        private readonly CookieContainer Cookies;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseUrl = "https://www.joinkfa.com";

        /// <summary>Nexacro 인증 사용자 ID</summary>
        public string? NexacroUserId { get; set; }

        /// <summary>Nexacro 인증 secret</summary>
        public string? NexacroSecret { get; set; }

        /// <summary>Nexacro 인증 정보 존재 여부</summary>
        public bool HasNexacroAuth => !string.IsNullOrEmpty(NexacroUserId) && !string.IsNullOrEmpty(NexacroSecret);

        public KfaApiClient()
        {
            Cookies = new CookieContainer();

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                CookieContainer = Cookies
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
        /// 인증 쿠키 설정 (state + JSESSIONID)
        /// </summary>
        public void SetAuthCookies(string? jsessionId = null)
        {
            if (string.IsNullOrEmpty(NexacroSecret))
            {
                return;
            }

            var baseUri = new Uri(BaseUrl);
            var stateValue = $"secret%3D{NexacroSecret}%26returnUrl%3Dhttps%3A%2F%2Fwww.joinkfa.com";
            Cookies.Add(baseUri, new Cookie("state", stateValue));

            if (!string.IsNullOrEmpty(jsessionId))
            {
                Cookies.Add(baseUri, new Cookie("JSESSIONID", jsessionId));
            }
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
        /// 경기 일정/결과 조회 (state 쿠키 인증 필요)
        /// </summary>
        public async Task<JsonElement?> GetMatchSingleListAsync(string matchIdx, string yearMonth = "")
        {
            var parameters = new Dictionary<string, string>
            {
                ["v_CURPAGENUM"] = "1",
                ["v_ROWCOUNTPERPAGE"] = "1000",
                ["v_MATCH_IDX"] = matchIdx,
                ["v_YEAR_MONTH"] = yearMonth,
                ["v_TEAMID"] = "",
                ["v_USER_ID"] = "",
                ["v_ORDERBY"] = ""
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

        /// <summary>
        /// 경기 상세 조회 (Nexacro SEARCH00.do)
        /// </summary>
        public async Task<Dictionary<string, List<Dictionary<string, string>>>?> GetMatchDetailAsync(
            string matchIdx, string singleIdx)
        {
            if (!HasNexacroAuth)
            {
                return null;
            }

            return await PostNexacroXmlAsync(matchIdx, singleIdx);
        }

        #region HTTP Helper

        private async Task<Dictionary<string, List<Dictionary<string, string>>>?> PostNexacroXmlAsync(
            string matchIdx, string singleIdx)
        {
            try
            {
                var xml = NexacroHelper.BuildMatchDetailRequest(
                    matchIdx, singleIdx, NexacroUserId!, NexacroSecret!);

                var content = new StringContent(xml, Encoding.UTF8, "text/xml");

                using var request = new HttpRequestMessage(HttpMethod.Post,
                    "/generate/MAP_04_001/SEARCH00.do?CALL_TYPE=NEXACRO");
                request.Content = content;
                request.Headers.Add("Gp_empl_id",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(NexacroUserId!)));

                var response = await Client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseXml = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseXml))
                {
                    return null;
                }

                return NexacroHelper.ParseResponse(responseXml);
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"[HTTP ERROR] SEARCH00.do: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[NEXACRO ERROR] SEARCH00.do: {ex.Message}");
                return null;
            }
        }

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
