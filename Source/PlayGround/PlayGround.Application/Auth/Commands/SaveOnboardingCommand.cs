using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 온보딩 프로필 저장 유즈케이스
    /// </summary>
    public class SaveOnboardingCommand
    {
        private readonly IAuthRepository Repository;

        public SaveOnboardingCommand(IAuthRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<SaveOnboardingResult>> ExecuteAsync(Guid userId, SaveOnboardingRequest request)
        {
            Debug.Assert(request != null, "SaveOnboardingRequest cannot be null");

            if (userId == Guid.Empty)
            {
                return Result<SaveOnboardingResult>.Error(ErrorCode.Unauthorized, "Invalid user");
            }

            var success = await Repository.SaveOnboardingProfileAsync(
                userId,
                request.ChildName,
                request.SportType,
                request.AgeGroup,
                request.Region);

            if (!success)
            {
                Debug.Assert(false, $"Failed to save onboarding profile for user: {userId}");
                return Result<SaveOnboardingResult>.Error(ErrorCode.DatabaseError, "Failed to save onboarding profile");
            }

            return Result<SaveOnboardingResult>.Success(new SaveOnboardingResult
            {
                UserId = userId
            });
        }
    }

    public class SaveOnboardingRequest
    {
        public string? ChildName { get; set; }
        public string? SportType { get; set; }
        public string? AgeGroup { get; set; }
        public string? Region { get; set; }
        public string? TeamName { get; set; }
    }

    public class SaveOnboardingResult
    {
        public Guid UserId { get; set; }
    }
}
