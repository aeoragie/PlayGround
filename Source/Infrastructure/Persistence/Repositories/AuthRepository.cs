using System.Diagnostics;
using Microsoft.Extensions.Options;
using Main.Entities;
using Main.Procedures;
using PlayGround.Application.Interfaces;
using PlayGround.Infrastructure.Database;
using PlayGround.Infrastructure.Database.Base;

namespace PlayGround.Persistence.Repositories
{
    /// <summary>
    /// 인증 리포지토리 구현
    /// </summary>
    public class AuthRepository : RepositoryBase, IAuthRepository
    {
        public override DatabaseTypes Database => DatabaseTypes.Main;

        public AuthRepository(IOptions<DatabaseConfiguration> options) : base(options)
        {
        }

        public async Task<UsersEntity?> GetUserByEmailAsync(string email)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be null or empty");

            var procedure = new UspGetUserByEmail(this) { Email = email };
            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? result.Value : null;
        }

        public async Task<UsersEntity?> GetUserByIdAsync(Guid userId)
        {
            Debug.Assert(userId != Guid.Empty, "UserId cannot be empty");

            var procedure = new UspGetUser(this) { UserId = userId };
            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? result.Value : null;
        }

        public async Task<UsersEntity?> CreateUserByEmailAsync(string email, string passwordHash, string fullName, string userRole)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be null or empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(passwordHash), "PasswordHash cannot be null or empty");

            var procedure = new UspCreateUserByEmail(this)
            {
                Email = email,
                PasswordHash = passwordHash,
                FullName = fullName,
                UserRole = userRole
            };

            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? result.Value : null;
        }

        public async Task<UsersEntity?> CreateUserWithSocialAccountAsync(
            string email, string fullName, string? profileImageUrl,
            string provider, string providerUserId,
            string? ipAddress, string? userAgent)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(provider), "Provider cannot be null or empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(providerUserId), "ProviderUserId cannot be null or empty");

            var procedure = new UspCreateUserWithSocialAccount(this)
            {
                Email = email,
                FullName = fullName,
                ProfileImageUrl = profileImageUrl,
                Provider = provider,
                ProviderUserId = providerUserId,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? result.Value : null;
        }

        public async Task<RefreshTokenResult?> CreateRefreshTokenAsync(
            Guid userId, string tokenHash, string? deviceInfo, string? ipAddress, DateTime expiresAt)
        {
            Debug.Assert(userId != Guid.Empty, "UserId cannot be empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(tokenHash), "TokenHash cannot be null or empty");

            var procedure = new UspCreateRefreshToken(this)
            {
                UserId = userId,
                Token = tokenHash,
                DeviceInfo = deviceInfo,
                IpAddress = ipAddress,
                ExpiresAt = expiresAt
            };

            var result = await procedure.SingleAsync<RefreshTokenResult>();

            return result.IsSuccess && result.HasValue ? result.Value : null;
        }

        public async Task<bool> UpdateLastLoginAsync(
            Guid userId, string? ipAddress, string? userAgent,
            string? deviceType, Guid? refreshTokenId, DateTime? sessionExpiresAt)
        {
            Debug.Assert(userId != Guid.Empty, "UserId cannot be empty");

            var procedure = new UspUpdateLastLogin(this)
            {
                UserId = userId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DeviceType = deviceType,
                RefreshTokenId = refreshTokenId,
                SessionExpiresAt = sessionExpiresAt
            };

            var result = await procedure.ExecuteAsync();

            return result.IsSuccess;
        }

        public async Task<UserRefreshTokenRow?> GetValidRefreshTokenAsync(string tokenHash)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(tokenHash), "TokenHash cannot be null or empty");

            var sql = @"SELECT
                [TokenId], [UserId], [Token], [IsRevoked], [ExpiresAt], [CreatedAt]
            FROM [dbo].[UserRefreshTokens] WITH (NOLOCK)
            WHERE [Token] = @TokenHash
                AND [IsRevoked] = 0
                AND [ExpiresAt] > GETUTCDATE();";

            var result = await QuerySingleOrDefaultAsync<UserRefreshTokenRow>(sql, new { TokenHash = tokenHash });

            return result.IsSuccess ? result.Value : null;
        }

        public async Task<bool> RevokeRefreshTokenAsync(Guid tokenId, string reason)
        {
            Debug.Assert(tokenId != Guid.Empty, "TokenId cannot be empty");

            var sql = @"UPDATE [dbo].[UserRefreshTokens]
            SET [IsRevoked] = 1,
                [RevokedAt] = GETUTCDATE(),
                [RevokedReason] = @Reason
            WHERE [TokenId] = @TokenId
                AND [IsRevoked] = 0;";

            var result = await ExecuteAsync(sql, new { TokenId = tokenId, Reason = reason });

            return result.IsSuccess && result.Value > 0;
        }
    }
}
