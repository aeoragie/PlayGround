using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 소셜 로그인 유즈케이스
    /// </summary>
    public class LoginBySocialCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;

        private const int RefreshTokenExpirationDays = 7;

        public LoginBySocialCommand(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<LoginResult>> ExecuteAsync(
            SocialLoginRequest request, string? ipAddress, string? userAgent)
        {
            Debug.Assert(request != null, "SocialLoginRequest cannot be null");

            if (string.IsNullOrWhiteSpace(request.Provider))
            {
                return Result<LoginResult>.Error(ErrorCode.MissingRequired, "Provider is required");
            }

            if (string.IsNullOrWhiteSpace(request.ProviderUserId))
            {
                return Result<LoginResult>.Error(ErrorCode.MissingRequired, "ProviderUserId is required");
            }

            // 기존 소셜 연동 사용자 조회
            var user = await Repository.GetUserWithSocialAccountAsync(
                request.Provider, request.ProviderUserId);

            // 없으면 새 사용자 생성
            if (user == null)
            {
                user = await Repository.CreateUserWithSocialAccountAsync(
                    request.Email ?? string.Empty,
                    request.FullName ?? "User",
                    request.ProfileImageUrl,
                    request.Provider,
                    request.ProviderUserId,
                    ipAddress,
                    userAgent);

                if (user == null)
                {
                    Debug.Assert(false, "Failed to create social user");
                    return Result<LoginResult>.Error(ErrorCode.DatabaseError, "Failed to create user");
                }
            }

            // 계정 상태 확인
            if (user.UserStatus != "Active")
            {
                return Result<LoginResult>.Error(ErrorCode.AccountDisabled, "Account is not active");
            }

            // JWT Access Token 생성
            var accessToken = TokenService.GenerateAccessToken(
                user.UserId, user.Email, user.FullName, user.UserRole, user.ProfileImageUrl);

            // Refresh Token 생성
            var rawRefreshToken = TokenService.GenerateRefreshToken();
            var refreshTokenHash = TokenService.HashToken(rawRefreshToken);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

            var tokenResult = await Repository.CreateRefreshTokenAsync(
                user.UserId, refreshTokenHash, userAgent, ipAddress, refreshTokenExpiry);

            if (tokenResult == null)
            {
                Debug.Assert(false, "Failed to create refresh token");
                return Result<LoginResult>.Error(ErrorCode.DatabaseError, "Failed to create session");
            }

            // 로그인 기록 업데이트
            await Repository.UpdateLastLoginAsync(
                user.UserId, ipAddress, userAgent, null, tokenResult.TokenId, refreshTokenExpiry);

            return Result<LoginResult>.Success(new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                ExpiresIn = 900,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                UserRole = user.UserRole,
                ProfileImageUrl = user.ProfileImageUrl
            });
        }
    }

    public class SocialLoginRequest
    {
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
