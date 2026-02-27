namespace PlayGround.Domain.SubDomains.Auth
{
    /// <summary>
    /// 인증 도메인 사용자 모델 (Application 레이어에서 사용)
    /// </summary>
    public class UserModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string UserStatus { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public DateTime? LockoutEndAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
