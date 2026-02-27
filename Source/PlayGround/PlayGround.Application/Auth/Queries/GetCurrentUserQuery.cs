using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Queries
{
    /// <summary>
    /// 현재 로그인 사용자 정보 조회 유즈케이스
    /// </summary>
    public class GetCurrentUserQuery
    {
        private readonly IAuthRepository Repository;

        public GetCurrentUserQuery(IAuthRepository repository)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Result<CurrentUserResult>> ExecuteAsync(Guid userId)
        {
            Debug.Assert(userId != Guid.Empty, "UserId cannot be empty");

            if (userId == Guid.Empty)
            {
                return Result<CurrentUserResult>.Error(ErrorCode.InvalidInput, "UserId is required");
            }

            var user = await Repository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return Result<CurrentUserResult>.Error(ErrorCode.NotFound, "User not found");
            }

            return Result<CurrentUserResult>.Success(new CurrentUserResult
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                NickName = user.NickName,
                ProfileImageUrl = user.ProfileImageUrl,
                UserRole = user.UserRole,
                UserStatus = user.UserStatus,
                EmailConfirmed = user.EmailConfirmed,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            });
        }
    }

    public class CurrentUserResult
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string UserStatus { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
