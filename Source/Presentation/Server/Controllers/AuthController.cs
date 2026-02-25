using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayGround.Application.Auth.Commands;
using PlayGround.Application.Auth.Queries;
using PlayGround.Application.Interfaces;
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

        private const string RefreshTokenCookieName = "refreshToken";

        public AuthController(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
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
