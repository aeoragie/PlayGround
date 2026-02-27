using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 이메일 인증 코드 발송 유즈케이스
    /// </summary>
    public class SendEmailVerificationCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IEmailService EmailService;

        public SendEmailVerificationCommand(IAuthRepository repository, IEmailService emailService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            EmailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<Result<SendVerificationResult>> ExecuteAsync(SendVerificationRequest request, string? ipAddress)
        {
            Debug.Assert(request != null, "SendVerificationRequest cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<SendVerificationResult>.Error(ErrorCode.MissingRequired, "Email is required");
            }

            var verification = await Repository.CreateEmailVerificationAsync(
                request.Email, request.Purpose ?? "SignUp", ipAddress);

            if (verification == null)
            {
                Debug.Assert(false, "Failed to create email verification");
                return Result<SendVerificationResult>.Error(ErrorCode.DatabaseError, "Failed to create verification");
            }

            await EmailService.SendVerificationCodeAsync(
                request.Email, verification.VerificationCode, request.Purpose ?? "SignUp");

            return Result<SendVerificationResult>.Success(new SendVerificationResult
            {
                VerificationId = verification.VerificationId,
                ExpiresAt = verification.ExpiresAt
            });
        }
    }

    public class SendVerificationRequest
    {
        public string Email { get; set; } = string.Empty;
        public string? Purpose { get; set; }
    }

    public class SendVerificationResult
    {
        public Guid VerificationId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
