using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 이메일 인증 코드 확인 유즈케이스
    /// </summary>
    public class VerifyEmailCommand
    {
        private readonly IAuthRepository Repository;

        public VerifyEmailCommand(IAuthRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<VerifyEmailResult>> ExecuteAsync(VerifyEmailRequest request)
        {
            Debug.Assert(request != null, "VerifyEmailRequest cannot be null");

            if (request.VerificationId == Guid.Empty)
            {
                return Result<VerifyEmailResult>.Error(ErrorCode.MissingRequired, "VerificationId is required");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Result<VerifyEmailResult>.Error(ErrorCode.MissingRequired, "Verification code is required");
            }

            var success = await Repository.VerifyEmailAsync(request.VerificationId, request.Code);

            if (!success)
            {
                return Result<VerifyEmailResult>.Error(ErrorCode.InvalidInput, "Invalid or expired verification code");
            }

            return Result<VerifyEmailResult>.Success(new VerifyEmailResult { Verified = true });
        }
    }

    public class VerifyEmailRequest
    {
        public Guid VerificationId { get; set; }
        public string Code { get; set; } = string.Empty;
    }

    public class VerifyEmailResult
    {
        public bool Verified { get; set; }
    }
}
