namespace PlayGround.Application.Interfaces
{
    /// <summary>
    /// JWT 토큰 서비스 인터페이스 (Server에서 구현)
    /// </summary>
    public interface IJwtTokenService
    {
        string GenerateAccessToken(Guid userId, string email, string fullName, string role, string? avatarUrl);
        string GenerateRefreshToken();
        string HashToken(string token);
    }
}
