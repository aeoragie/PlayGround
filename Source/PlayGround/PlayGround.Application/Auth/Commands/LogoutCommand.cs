using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 로그아웃 유즈케이스
    /// </summary>
    public class LogoutCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;

        public LogoutCommand(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result> ExecuteAsync(string refreshToken)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(refreshToken), "RefreshToken cannot be null or empty");

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Result.Error(ErrorCode.MissingRequired, "Refresh token is required");
            }

            var tokenHash = TokenService.HashToken(refreshToken);
            var storedToken = await Repository.GetValidRefreshTokenAsync(tokenHash);

            if (storedToken == null)
            {
                // 이미 무효화된 토큰이라도 로그아웃 성공 처리
                return Result.Success();
            }

            var revoked = await Repository.RevokeRefreshTokenAsync(storedToken.TokenId, "UserLogout");
            if (!revoked)
            {
                Debug.Assert(false, "Failed to revoke refresh token");
                return Result.Error(ErrorCode.DatabaseError, "Failed to revoke session");
            }

            return Result.Success();
        }
    }
}
