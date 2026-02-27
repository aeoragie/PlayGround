namespace PlayGround.Application.Interfaces
{
    /// <summary>
    /// 이메일 발송 인터페이스
    /// </summary>
    public interface IEmailService
    {
        Task SendVerificationCodeAsync(string email, string code, string purpose);
        Task SendPasswordResetAsync(string email, string resetToken);
    }
}
