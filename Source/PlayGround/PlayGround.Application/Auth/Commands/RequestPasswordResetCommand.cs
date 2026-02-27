using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 비밀번호 재설정 요청 유즈케이스 (항상 200 반환 — 보안)
    /// </summary>
    public class RequestPasswordResetCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;
        private readonly IEmailService EmailService;

        public RequestPasswordResetCommand(
            IAuthRepository repository, IJwtTokenService tokenService, IEmailService emailService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<Result<PasswordResetRequestResult>> ExecuteAsync(PasswordResetRequestRequest request, string? ipAddress)
        {
            Debug.Assert(request != null, "PasswordResetRequestRequest cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<PasswordResetRequestResult>.Error(ErrorCode.MissingRequired, "Email is required");
            }

            // 토큰 생성
            var rawToken = TokenService.GenerateRefreshToken();
            var tokenHash = TokenService.HashToken(rawToken);

            var tokenResult = await Repository.CreatePasswordResetTokenAsync(
                request.Email, tokenHash, ipAddress);

            // 사용자 존재 여부와 관계없이 이메일 발송 시도
            if (tokenResult != null)
            {
                await EmailService.SendPasswordResetAsync(request.Email, rawToken);
            }

            // 항상 성공 반환 (보안: 사용자 존재 여부 노출 방지)
            return Result<PasswordResetRequestResult>.Success(new PasswordResetRequestResult
            {
                Message = "If an account with that email exists, a reset link has been sent."
            });
        }
    }

    public class PasswordResetRequestRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class PasswordResetRequestResult
    {
        public string Message { get; set; } = string.Empty;
    }
}
