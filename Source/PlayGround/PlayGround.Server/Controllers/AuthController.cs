using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGround.Application.Auth.Commands;
using PlayGround.Application.Auth.Queries;
using PlayGround.Application.Interfaces;
using PlayGround.Server.Services;
using PlayGround.Shared.DTOs;
using PlayGround.Shared.Result;

namespace PlayGround.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;
        private readonly IEmailService EmailService;
        private readonly OAuthService OAuth;
        private readonly IConfiguration Configuration;

        private const string RefreshTokenCookieName = "refreshToken";

        public AuthController(
            IAuthRepository repository, IJwtTokenService tokenService,
            IEmailService emailService, OAuthService oAuthService,
            IConfiguration configuration)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            OAuth = oAuthService ?? throw new ArgumentNullException(nameof(oAuthService));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// 이메일 회원가입
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var command = new RegisterByEmailCommand(Repository, TokenService);
            var result = await command.ExecuteAsync(request);

            return result.Match<IActionResult>(
                onSuccess: data => StatusCode(201, ApiResponse<RegisterResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 이메일 로그인
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();

            var command = new LoginByEmailCommand(Repository, TokenService);
            var result = await command.ExecuteAsync(request, ipAddress, userAgent);

            return result.Match<IActionResult>(
                onSuccess: data =>
                {
                    SetRefreshTokenCookie(data.RefreshToken);

                    return Ok(ApiResponse<object>.Success(new
                    {
                        data.AccessToken,
                        data.ExpiresIn,
                        data.UserId,
                        data.Email,
                        data.FullName,
                        data.UserRole,
                        data.ProfileImageUrl
                    }));
                },
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// Access Token 재발급
        /// </summary>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(ApiResponse<object>.Fail("Refresh token not found"));
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();

            var command = new RefreshTokenCommand(Repository, TokenService);
            var result = await command.ExecuteAsync(refreshToken, ipAddress, userAgent);

            return result.Match<IActionResult>(
                onSuccess: data =>
                {
                    SetRefreshTokenCookie(data.RefreshToken);

                    return Ok(ApiResponse<object>.Success(new
                    {
                        data.AccessToken,
                        data.ExpiresIn
                    }));
                },
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 로그아웃
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var command = new LogoutCommand(Repository, TokenService);
                await command.ExecuteAsync(refreshToken);
            }

            DeleteRefreshTokenCookie();

            return Ok(ApiResponse<object>.Success(new { Message = "Logged out successfully" }));
        }

        /// <summary>
        /// 현재 사용자 정보 조회
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid token"));
            }

            var query = new GetCurrentUserQuery(Repository);
            var result = await query.ExecuteAsync(userId);

            return result.Match<IActionResult>(
                onSuccess: data => Ok(ApiResponse<CurrentUserResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 온보딩 프로필 저장
        /// </summary>
        [Authorize]
        [HttpPost("onboarding")]
        public async Task<IActionResult> SaveOnboarding([FromBody] SaveOnboardingRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid token"));
            }

            var command = new SaveOnboardingCommand(Repository);
            var result = await command.ExecuteAsync(userId, request);

            return result.Match<IActionResult>(
                onSuccess: data => Ok(ApiResponse<SaveOnboardingResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 이메일 인증 코드 발송
        /// </summary>
        [HttpPost("send-verification")]
        public async Task<IActionResult> SendVerification([FromBody] SendVerificationRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var command = new SendEmailVerificationCommand(Repository, EmailService);
            var result = await command.ExecuteAsync(request, ipAddress);

            return result.Match<IActionResult>(
                onSuccess: data => Ok(ApiResponse<SendVerificationResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 이메일 인증 코드 확인
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            var command = new VerifyEmailCommand(Repository);
            var result = await command.ExecuteAsync(request);

            return result.Match<IActionResult>(
                onSuccess: data => Ok(ApiResponse<VerifyEmailResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 비밀번호 재설정 요청
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] PasswordResetRequestRequest request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var command = new RequestPasswordResetCommand(Repository, TokenService, EmailService);
            var result = await command.ExecuteAsync(request, ipAddress);

            return result.Match<IActionResult>(
                onSuccess: data => Ok(ApiResponse<PasswordResetRequestResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 비밀번호 재설정
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var command = new ResetPasswordCommand(Repository, TokenService);
            var result = await command.ExecuteAsync(request);

            return result.Match<IActionResult>(
                onSuccess: data => Ok(ApiResponse<ResetPasswordResult>.Success(data)),
                onFailure: info => ToErrorResponse(info));
        }

        /// <summary>
        /// 소셜 로그인 리디렉션
        /// </summary>
        [HttpGet("social/{provider}")]
        public IActionResult SocialLogin(string provider)
        {
            try
            {
                var state = TokenService.GenerateRefreshToken();
                HttpContext.Session.SetString("oauth_state", state);

                var authUrl = OAuth.GetAuthorizationUrl(provider, state);
                return Redirect(authUrl);
            }
            catch (ArgumentException)
            {
                return BadRequest(ApiResponse<object>.Fail($"Unsupported provider: {provider}"));
            }
        }

        /// <summary>
        /// 소셜 로그인 콜백
        /// </summary>
        [HttpGet("social/{provider}/callback")]
        public async Task<IActionResult> SocialCallback(string provider, [FromQuery] string code, [FromQuery] string? state)
        {
            var clientBaseUrl = Configuration["ClientBaseUrl"] ?? "/";

            if (string.IsNullOrWhiteSpace(code))
            {
                return Redirect($"{clientBaseUrl}auth/social-callback?error=NoCode");
            }

            var userInfo = await OAuth.GetUserInfoAsync(provider, code);
            if (userInfo == null)
            {
                return Redirect($"{clientBaseUrl}auth/social-callback?error=ProviderError");
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();

            var command = new LoginBySocialCommand(Repository, TokenService);
            var loginRequest = new SocialLoginRequest
            {
                Provider = userInfo.Provider,
                ProviderUserId = userInfo.ProviderUserId,
                Email = userInfo.Email,
                FullName = userInfo.FullName,
                ProfileImageUrl = userInfo.ProfileImageUrl
            };

            var result = await command.ExecuteAsync(loginRequest, ipAddress, userAgent);

            return result.Match<IActionResult>(
                onSuccess: data =>
                {
                    SetRefreshTokenCookie(data.RefreshToken);
                    return Redirect($"{clientBaseUrl}auth/social-callback?success=true");
                },
                onFailure: info =>
                {
                    return Redirect($"{clientBaseUrl}auth/social-callback?error={Uri.EscapeDataString(info.Message)}");
                });
        }

        #region Helper Methods

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
        }

        private void DeleteRefreshTokenCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        private IActionResult ToErrorResponse(ResultInfo info)
        {
            var statusCode = info.DetailCode switch
            {
                _ when info.DetailCode == ErrorCode.InvalidCredentials => 401,
                _ when info.DetailCode == ErrorCode.Unauthorized => 401,
                _ when info.DetailCode == ErrorCode.InvalidToken => 401,
                _ when info.DetailCode == ErrorCode.InvalidRefreshToken => 401,
                _ when info.DetailCode == ErrorCode.RefreshTokenExpired => 401,
                _ when info.DetailCode == ErrorCode.TokenExpired => 401,
                _ when info.DetailCode == ErrorCode.Forbidden => 403,
                _ when info.DetailCode == ErrorCode.AccountLocked => 423,
                _ when info.DetailCode == ErrorCode.AccountDisabled => 403,
                _ when info.DetailCode == ErrorCode.AccountNotVerified => 403,
                _ when info.DetailCode == ErrorCode.NotFound => 404,
                _ when info.DetailCode == ErrorCode.AlreadyExists => 409,
                _ when info.DetailCode is ErrorCode code && code.IsClientError => 400,
                _ => 500
            };

            return StatusCode(statusCode, ApiResponse<object>.Fail(info.Message));
        }

        #endregion
    }
}
