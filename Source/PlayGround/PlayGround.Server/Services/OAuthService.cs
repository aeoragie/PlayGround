using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PlayGround.Server.Services
{
    /// <summary>
    /// OAuth Provider별 인증 처리 서비스
    /// </summary>
    public class OAuthService
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly IConfiguration Configuration;

        public OAuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// OAuth 인증 URL 생성
        /// </summary>
        public string GetAuthorizationUrl(string provider, string state)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(provider), "Provider cannot be null or empty");

            return provider.ToLower() switch
            {
                "google" => GetGoogleAuthUrl(state),
                "kakao" => GetKakaoAuthUrl(state),
                _ => throw new ArgumentException($"Unsupported provider: {provider}")
            };
        }

        /// <summary>
        /// OAuth 코드로 사용자 정보 조회
        /// </summary>
        public async Task<OAuthUserInfo?> GetUserInfoAsync(string provider, string code)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(provider), "Provider cannot be null or empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(code), "Code cannot be null or empty");

            return provider.ToLower() switch
            {
                "google" => await GetGoogleUserInfoAsync(code),
                "kakao" => await GetKakaoUserInfoAsync(code),
                _ => throw new ArgumentException($"Unsupported provider: {provider}")
            };
        }

        #region Google

        private string GetGoogleAuthUrl(string state)
        {
            var clientId = Configuration["OAuth:Google:ClientId"];
            var redirectUri = Configuration["OAuth:Google:RedirectUri"];

            Debug.Assert(!string.IsNullOrWhiteSpace(clientId), "OAuth:Google:ClientId is not configured");

            return $"https://accounts.google.com/o/oauth2/v2/auth" +
                   $"?client_id={Uri.EscapeDataString(clientId!)}" +
                   $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                   $"&response_type=code" +
                   $"&scope={Uri.EscapeDataString("openid email profile")}" +
                   $"&state={Uri.EscapeDataString(state)}" +
                   $"&access_type=offline";
        }

        private async Task<OAuthUserInfo?> GetGoogleUserInfoAsync(string code)
        {
            var clientId = Configuration["OAuth:Google:ClientId"];
            var clientSecret = Configuration["OAuth:Google:ClientSecret"];
            var redirectUri = Configuration["OAuth:Google:RedirectUri"];

            var client = HttpClientFactory.CreateClient();

            // 코드 → 토큰 교환
            var tokenRequest = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!,
                ["redirect_uri"] = redirectUri!,
                ["grant_type"] = "authorization_code"
            };

            var tokenResponse = await client.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest));

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
            var accessToken = tokenJson.GetProperty("access_token").GetString();

            // 토큰 → 유저 정보
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");

            if (!userResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var userJson = await userResponse.Content.ReadFromJsonAsync<JsonElement>();

            return new OAuthUserInfo
            {
                Provider = "Google",
                ProviderUserId = userJson.GetProperty("id").GetString() ?? string.Empty,
                Email = userJson.TryGetProperty("email", out var email) ? email.GetString() : null,
                FullName = userJson.TryGetProperty("name", out var name) ? name.GetString() : null,
                ProfileImageUrl = userJson.TryGetProperty("picture", out var picture) ? picture.GetString() : null
            };
        }

        #endregion

        #region Kakao

        private string GetKakaoAuthUrl(string state)
        {
            var clientId = Configuration["OAuth:Kakao:ClientId"];
            var redirectUri = Configuration["OAuth:Kakao:RedirectUri"];

            Debug.Assert(!string.IsNullOrWhiteSpace(clientId), "OAuth:Kakao:ClientId is not configured");

            return $"https://kauth.kakao.com/oauth/authorize" +
                   $"?client_id={Uri.EscapeDataString(clientId!)}" +
                   $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                   $"&response_type=code" +
                   $"&state={Uri.EscapeDataString(state)}";
        }

        private async Task<OAuthUserInfo?> GetKakaoUserInfoAsync(string code)
        {
            var clientId = Configuration["OAuth:Kakao:ClientId"];
            var clientSecret = Configuration["OAuth:Kakao:ClientSecret"];
            var redirectUri = Configuration["OAuth:Kakao:RedirectUri"];

            var client = HttpClientFactory.CreateClient();

            // 코드 → 토큰 교환
            var tokenRequest = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = clientId!,
                ["redirect_uri"] = redirectUri!,
                ["code"] = code
            };

            if (!string.IsNullOrWhiteSpace(clientSecret))
            {
                tokenRequest["client_secret"] = clientSecret;
            }

            var tokenResponse = await client.PostAsync(
                "https://kauth.kakao.com/oauth/token",
                new FormUrlEncodedContent(tokenRequest));

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
            var accessToken = tokenJson.GetProperty("access_token").GetString();

            // 토큰 → 유저 정보
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userResponse = await client.GetAsync("https://kapi.kakao.com/v2/user/me");

            if (!userResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var userJson = await userResponse.Content.ReadFromJsonAsync<JsonElement>();
            var kakaoId = userJson.GetProperty("id").GetInt64().ToString();

            string? email = null;
            string? fullName = null;
            string? profileImageUrl = null;

            if (userJson.TryGetProperty("kakao_account", out var account))
            {
                email = account.TryGetProperty("email", out var e) ? e.GetString() : null;

                if (account.TryGetProperty("profile", out var profile))
                {
                    fullName = profile.TryGetProperty("nickname", out var n) ? n.GetString() : null;
                    profileImageUrl = profile.TryGetProperty("profile_image_url", out var p) ? p.GetString() : null;
                }
            }

            return new OAuthUserInfo
            {
                Provider = "Kakao",
                ProviderUserId = kakaoId,
                Email = email,
                FullName = fullName,
                ProfileImageUrl = profileImageUrl
            };
        }

        #endregion
    }

    public class OAuthUserInfo
    {
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
