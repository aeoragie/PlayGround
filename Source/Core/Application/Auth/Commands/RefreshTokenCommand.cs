using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// Refresh Token으로 Access Token 재발급 유즈케이스
    /// </summary>
    public class RefreshTokenCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;

        private const int RefreshTokenExpirationDays = 7;

        public RefreshTokenCommand(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<LoginResult>> ExecuteAsync(string refreshToken, string? ipAddress, string? userAgent)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(refreshToken), "RefreshToken cannot be null or empty");

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Result<LoginResult>.Error(ErrorCode.MissingRequired, "Refresh token is required");
            }

            // Refresh Token 해시로 조회
            var tokenHash = TokenService.HashToken(refreshToken);
            var storedToken = await Repository.GetValidRefreshTokenAsync(tokenHash);

            if (storedToken == null)
            {
                return Result<LoginResult>.Error(ErrorCode.InvalidRefreshToken, "Invalid refresh token");
            }

            if (storedToken.IsRevoked)
            {
                return Result<LoginResult>.Error(ErrorCode.InvalidRefreshToken, "Refresh token has been revoked");
            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                return Result<LoginResult>.Error(ErrorCode.RefreshTokenExpired, "Refresh token has expired");
            }

            // 사용자 조회
            var user = await Repository.GetUserByIdAsync(storedToken.UserId);
            if (user == null)
            {
                Debug.Assert(false, $"User not found for valid refresh token: {storedToken.UserId}");
                return Result<LoginResult>.Error(ErrorCode.NotFound, "User not found");
            }

            if (user.UserStatus != "Active")
            {
                return Result<LoginResult>.Error(ErrorCode.AccountDisabled, "Account is not active");
            }

            // 새 Access Token 생성
            var accessToken = TokenService.GenerateAccessToken(
                user.UserId, user.Email, user.FullName, user.UserRole, user.ProfileImageUrl);

            // 새 Refresh Token 생성 (Rotation)
            var newRawRefreshToken = TokenService.GenerateRefreshToken();
            var newRefreshTokenHash = TokenService.HashToken(newRawRefreshToken);
            var newExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

            var newTokenResult = await Repository.CreateRefreshTokenAsync(
                user.UserId, newRefreshTokenHash, userAgent, ipAddress, newExpiry);

            if (newTokenResult == null)
            {
                Debug.Assert(false, "Failed to create new refresh token");
                return Result<LoginResult>.Error(ErrorCode.DatabaseError, "Failed to refresh session");
            }

            return Result<LoginResult>.Success(new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = newRawRefreshToken,
                ExpiresIn = 900,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                UserRole = user.UserRole,
                ProfileImageUrl = user.ProfileImageUrl
            });
        }
    }
}
