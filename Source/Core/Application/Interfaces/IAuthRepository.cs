using PlayGround.Domain.SubDomains.Auth;

namespace PlayGround.Application.Interfaces
{
    /// <summary>
    /// 인증 리포지토리 인터페이스 (Persistence에서 구현)
    /// </summary>
    public interface IAuthRepository
    {
        Task<UserModel?> GetUserByEmailAsync(string email);
        Task<UserModel?> GetUserByIdAsync(Guid userId);
        Task<UserModel?> CreateUserByEmailAsync(string email, string passwordHash, string fullName, string userRole);
        Task<UserModel?> CreateUserWithSocialAccountAsync(string email, string fullName, string? profileImageUrl, string provider, string providerUserId, string? ipAddress, string? userAgent);
        Task<RefreshTokenResult?> CreateRefreshTokenAsync(Guid userId, string tokenHash, string? deviceInfo, string? ipAddress, DateTime expiresAt);
        Task<bool> UpdateLastLoginAsync(Guid userId, string? ipAddress, string? userAgent, string? deviceType, Guid? refreshTokenId, DateTime? sessionExpiresAt);
        Task<UserRefreshTokenRow?> GetValidRefreshTokenAsync(string tokenHash);
        Task<bool> RevokeRefreshTokenAsync(Guid tokenId, string reason);
    }

    /// <summary>
    /// Refresh Token 생성 결과
    /// </summary>
    public class RefreshTokenResult
    {
        public Guid TokenId { get; set; }
    }

    /// <summary>
    /// Refresh Token 조회 결과
    /// </summary>
    public class UserRefreshTokenRow
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsRevoked { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
