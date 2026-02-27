using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 비밀번호 재설정 유즈케이스
    /// </summary>
    public class ResetPasswordCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;

        public ResetPasswordCommand(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<ResetPasswordResult>> ExecuteAsync(ResetPasswordRequest request)
        {
            Debug.Assert(request != null, "ResetPasswordRequest cannot be null");

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return Result<ResetPasswordResult>.Error(ErrorCode.MissingRequired, "Reset token is required");
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            {
                return Result<ResetPasswordResult>.Error(ErrorCode.InvalidInput, "Password must be at least 8 characters");
            }

            var tokenHash = TokenService.HashToken(request.Token);
            var newPasswordHash = TokenService.HashToken(request.NewPassword);

            var success = await Repository.ResetPasswordAsync(tokenHash, newPasswordHash);

            if (!success)
            {
                return Result<ResetPasswordResult>.Error(ErrorCode.InvalidToken, "Invalid or expired reset token");
            }

            return Result<ResetPasswordResult>.Success(new ResetPasswordResult { Reset = true });
        }
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordResult
    {
        public bool Reset { get; set; }
    }
}
