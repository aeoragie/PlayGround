using NLog;
using PlayGround.Application.Interfaces;

namespace PlayGround.Persistence.Email
{
    /// <summary>
    /// 개발용 콘솔 이메일 서비스 (실제 메일 미발송)
    /// </summary>
    public class ConsoleEmailService : IEmailService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public Task SendVerificationCodeAsync(string email, string code, string purpose)
        {
            Logger.Info("[EMAIL] Verification code for {Email} (purpose: {Purpose}): {Code}",
                email, purpose, code);

            return Task.CompletedTask;
        }

        public Task SendPasswordResetAsync(string email, string resetToken)
        {
            Logger.Info("[EMAIL] Password reset token for {Email}: {Token}",
                email, resetToken);

            return Task.CompletedTask;
        }
    }
}
