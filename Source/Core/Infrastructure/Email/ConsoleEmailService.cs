using Microsoft.Extensions.Logging;
using PlayGround.Application.Interfaces;

namespace PlayGround.Infrastructure.Email
{
    /// <summary>
    /// 개발용 콘솔 이메일 서비스 (실제 메일 미발송)
    /// </summary>
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> Logger;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task SendVerificationCodeAsync(string email, string code, string purpose)
        {
            Logger.LogInformation(
                "[EMAIL] Verification code for {Email} (purpose: {Purpose}): {Code}",
                email, purpose, code);

            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(string email, string resetToken)
        {
            Logger.LogInformation(
                "[EMAIL] Password reset token for {Email}: {Token}",
                email, resetToken);

            return Task.CompletedTask;
        }
    }
}
