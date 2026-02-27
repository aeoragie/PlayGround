using System.Diagnostics;
using Microsoft.Extensions.Options;
using Main.Entities;
using Main.Procedures;
using PlayGround.Application.Interfaces;
using PlayGround.Domain.SubDomains.Auth;
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

        public async Task<UserModel?> GetUserByEmailAsync(string email)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be null or empty");

            var procedure = new UspGetUserByEmail(this) { Email = email };
            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? ToUserModel(result.Value) : null;
        }

        public async Task<UserModel?> GetUserByIdAsync(Guid userId)
        {
            Debug.Assert(userId != Guid.Empty, "UserId cannot be empty");

            var procedure = new UspGetUser(this) { UserId = userId };
            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? ToUserModel(result.Value) : null;
        }

        public async Task<UserModel?> CreateUserByEmailAsync(string email, string passwordHash, string fullName, string userRole)
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

            return result.IsSuccess && result.HasValue ? ToUserModel(result.Value) : null;
        }

        public async Task<UserModel?> CreateUserWithSocialAccountAsync(
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

            return result.IsSuccess && result.HasValue ? ToUserModel(result.Value) : null;
        }

        public async Task<UserModel?> GetUserWithSocialAccountAsync(string provider, string providerUserId)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(provider), "Provider cannot be null or empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(providerUserId), "ProviderUserId cannot be null or empty");

            var procedure = new UspGetUserWithSocialAccount(this)
            {
                Provider = provider,
                ProviderUserId = providerUserId
            };

            var result = await procedure.SingleAsync<UsersEntity>();

            return result.IsSuccess && result.HasValue ? ToUserModel(result.Value) : null;
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

        public async Task<bool> SaveOnboardingProfileAsync(
            Guid userId, string? childName, string? sportType, string? ageGroup, string? region)
        {
            Debug.Assert(userId != Guid.Empty, "UserId cannot be empty");

            var sql = @"UPDATE [dbo].[Users]
            SET
                [NickName] = ISNULL(@ChildName, [NickName]),
                [SportType] = ISNULL(@SportType, [SportType]),
                [AgeGroup] = ISNULL(@AgeGroup, [AgeGroup]),
                [Region] = ISNULL(@Region, [Region]),
                [UpdatedAt] = GETUTCDATE()
            WHERE [UserId] = @UserId
                AND [DeletedAt] IS NULL;";

            var result = await ExecuteAsync(sql, new { UserId = userId, ChildName = childName, SportType = sportType, AgeGroup = ageGroup, Region = region });

            return result.IsSuccess && result.Value > 0;
        }

        public async Task<EmailVerificationResult?> CreateEmailVerificationAsync(
            string email, string purpose, string? requestIp)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be null or empty");

            var sql = @"
            DECLARE @VerificationId UNIQUEIDENTIFIER = NEWID();
            DECLARE @Code VARCHAR(10);
            DECLARE @ExpiresAt DATETIME2 = DATEADD(MINUTE, 10, GETUTCDATE());

            SET @Code = RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS VARCHAR(6)), 6);

            UPDATE [dbo].[EmailVerifications]
            SET [ExpiresAt] = GETUTCDATE()
            WHERE [Email] = @Email
                AND [Purpose] = @Purpose
                AND [IsVerified] = 0
                AND [ExpiresAt] > GETUTCDATE();

            INSERT INTO [dbo].[EmailVerifications]
            ([VerificationId], [Email], [VerificationCode], [Purpose], [RequestIp], [AttemptCount], [IsVerified], [ExpiresAt], [CreatedAt])
            VALUES
            (@VerificationId, @Email, @Code, @Purpose, @RequestIp, 0, 0, @ExpiresAt, GETUTCDATE());

            SELECT @VerificationId AS [VerificationId], @Code AS [VerificationCode], @ExpiresAt AS [ExpiresAt];";

            var result = await QuerySingleOrDefaultAsync<EmailVerificationResult>(
                sql, new { Email = email, Purpose = purpose, RequestIp = requestIp });

            return result.IsSuccess ? result.Value : null;
        }

        public async Task<bool> VerifyEmailAsync(Guid verificationId, string code)
        {
            Debug.Assert(verificationId != Guid.Empty, "VerificationId cannot be empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(code), "Code cannot be null or empty");

            var sql = @"
            DECLARE @Email VARCHAR(255);
            DECLARE @IsValid BIT = 0;

            SELECT @Email = [Email], @IsValid = 1
            FROM [dbo].[EmailVerifications]
            WHERE [VerificationId] = @VerificationId
                AND [VerificationCode] = @Code
                AND [IsVerified] = 0
                AND [ExpiresAt] > GETUTCDATE()
                AND [AttemptCount] < 5;

            IF @IsValid = 0
            BEGIN
                UPDATE [dbo].[EmailVerifications]
                SET [AttemptCount] = [AttemptCount] + 1
                WHERE [VerificationId] = @VerificationId
                    AND [IsVerified] = 0;

                SELECT 0 AS [Result];
                RETURN;
            END

            UPDATE [dbo].[EmailVerifications]
            SET [IsVerified] = 1, [VerifiedAt] = GETUTCDATE()
            WHERE [VerificationId] = @VerificationId;

            UPDATE [dbo].[Users]
            SET [EmailConfirmed] = 1, [UpdatedAt] = GETUTCDATE()
            WHERE [Email] = @Email AND [DeletedAt] IS NULL;

            SELECT 1 AS [Result];";

            var result = await QuerySingleOrDefaultAsync<int>(
                sql, new { VerificationId = verificationId, Code = code });

            return result.IsSuccess && result.Value == 1;
        }

        public async Task<PasswordResetTokenResult?> CreatePasswordResetTokenAsync(
            string email, string tokenHash, string? requestIp)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(email), "Email cannot be null or empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(tokenHash), "TokenHash cannot be null or empty");

            var sql = @"
            DECLARE @UserId UNIQUEIDENTIFIER;
            DECLARE @TokenId UNIQUEIDENTIFIER = NEWID();
            DECLARE @ExpiresAt DATETIME2 = DATEADD(HOUR, 1, GETUTCDATE());

            SELECT @UserId = [UserId]
            FROM [dbo].[Users]
            WHERE [Email] = @Email AND [DeletedAt] IS NULL;

            IF @UserId IS NULL
            BEGIN
                SELECT NEWID() AS [TokenId];
                RETURN;
            END

            UPDATE [dbo].[PasswordResetTokens]
            SET [ExpiresAt] = GETUTCDATE()
            WHERE [UserId] = @UserId AND [UsedAt] IS NULL AND [ExpiresAt] > GETUTCDATE();

            INSERT INTO [dbo].[PasswordResetTokens]
            ([TokenId], [UserId], [TokenHash], [RequestIp], [ExpiresAt], [CreatedAt])
            VALUES
            (@TokenId, @UserId, @TokenHash, @RequestIp, @ExpiresAt, GETUTCDATE());

            SELECT @TokenId AS [TokenId];";

            var result = await QuerySingleOrDefaultAsync<PasswordResetTokenResult>(
                sql, new { Email = email, TokenHash = tokenHash, RequestIp = requestIp });

            return result.IsSuccess ? result.Value : null;
        }

        public async Task<bool> ResetPasswordAsync(string tokenHash, string newPasswordHash)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(tokenHash), "TokenHash cannot be null or empty");
            Debug.Assert(!string.IsNullOrWhiteSpace(newPasswordHash), "NewPasswordHash cannot be null or empty");

            var sql = @"
            DECLARE @UserId UNIQUEIDENTIFIER;

            SELECT @UserId = [UserId]
            FROM [dbo].[PasswordResetTokens]
            WHERE [TokenHash] = @TokenHash AND [UsedAt] IS NULL AND [ExpiresAt] > GETUTCDATE();

            IF @UserId IS NULL
            BEGIN
                SELECT 0 AS [Result];
                RETURN;
            END

            UPDATE [dbo].[PasswordResetTokens]
            SET [UsedAt] = GETUTCDATE()
            WHERE [TokenHash] = @TokenHash AND [UsedAt] IS NULL;

            UPDATE [dbo].[Users]
            SET [PasswordHash] = @NewPasswordHash, [UpdatedAt] = GETUTCDATE()
            WHERE [UserId] = @UserId AND [DeletedAt] IS NULL;

            UPDATE [dbo].[UserRefreshTokens]
            SET [IsRevoked] = 1, [RevokedAt] = GETUTCDATE(), [RevokedReason] = 'PasswordReset'
            WHERE [UserId] = @UserId AND [IsRevoked] = 0;

            SELECT 1 AS [Result];";

            var result = await QuerySingleOrDefaultAsync<int>(
                sql, new { TokenHash = tokenHash, NewPasswordHash = newPasswordHash });

            return result.IsSuccess && result.Value == 1;
        }

        private static UserModel ToUserModel(UsersEntity entity)
        {
            return new UserModel
            {
                UserId = entity.UserId,
                Email = entity.Email,
                FullName = entity.FullName,
                NickName = entity.NickName,
                PasswordHash = entity.PasswordHash,
                ProfileImageUrl = string.IsNullOrEmpty(entity.ProfileImageUrl) ? null : entity.ProfileImageUrl,
                UserRole = entity.UserRole,
                UserStatus = entity.UserStatus,
                EmailConfirmed = entity.EmailConfirmed,
                LockoutEndAt = entity.LockoutEndAt,
                LastLoginAt = entity.LastLoginAt,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
