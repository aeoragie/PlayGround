using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PlayGround.Shared.DTOs;

namespace PlayGround.Client.Services
{
    /// <summary>
    /// 인증 API 통신 서비스
    /// </summary>
    public class AuthService
    {
        private readonly HttpClient Http;

        private string? mAccessToken;

        public string? AccessToken => mAccessToken;
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(mAccessToken);

        public AuthService(HttpClient http)
        {
            Http = http ?? throw new ArgumentNullException(nameof(http));
        }

        /// <summary>
        /// 이메일 회원가입
        /// </summary>
        public async Task<ApiResponse<AuthRegisterResult>> RegisterAsync(
            string email, string password, string fullName, string? role)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be empty");

            var request = new { Email = email, Password = password, FullName = fullName, Role = role };
            var response = await Http.PostAsJsonAsync("api/auth/register", request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthRegisterResult>>();

            Debug.Assert(result != null, "Register response deserialization failed");
            return result ?? ApiResponse<AuthRegisterResult>.Fail("Unexpected error");
        }

        /// <summary>
        /// 이메일 로그인
        /// </summary>
        public async Task<ApiResponse<AuthLoginResult>> LoginAsync(string email, string password)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be empty");

            var request = new { Email = email, Password = password };
            var response = await Http.PostAsJsonAsync("api/auth/login", request);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthLoginResult>>();

            if (result?.IsSuccess == true && result.Data != null)
            {
                mAccessToken = result.Data.AccessToken;
            }

            Debug.Assert(result != null, "Login response deserialization failed");
            return result ?? ApiResponse<AuthLoginResult>.Fail("Unexpected error");
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        public async Task LogoutAsync()
        {
            SetAuthorizationHeader();

            try
            {
                await Http.PostAsync("api/auth/logout", null);
            }
            finally
            {
                mAccessToken = null;
                Http.DefaultRequestHeaders.Authorization = null;
            }
        }

        /// <summary>
        /// 현재 사용자 조회
        /// </summary>
        public async Task<ApiResponse<AuthCurrentUser>> GetCurrentUserAsync()
        {
            SetAuthorizationHeader();
            var response = await Http.GetAsync("api/auth/me");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthCurrentUser>>();

            Debug.Assert(result != null, "GetCurrentUser response deserialization failed");
            return result ?? ApiResponse<AuthCurrentUser>.Fail("Unexpected error");
        }

        /// <summary>
        /// Access Token 갱신 (RefreshToken은 HttpOnly 쿠키로 자동 전송)
        /// </summary>
        public async Task<bool> TryRefreshTokenAsync()
        {
            var response = await Http.PostAsync("api/auth/refresh", null);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<AuthRefreshResult>>();
            if (result?.IsSuccess == true && result.Data != null)
            {
                mAccessToken = result.Data.AccessToken;
                return true;
            }

            return false;
        }

        private void SetAuthorizationHeader()
        {
            if (!string.IsNullOrWhiteSpace(mAccessToken))
            {
                Http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", mAccessToken);
            }
        }
    }

    #region Client DTOs

    public class AuthRegisterResult
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }

    public class AuthLoginResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }

    public class AuthRefreshResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }

    public class AuthCurrentUser
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string UserStatus { get; set; } = string.Empty;
    }

    #endregion
}
