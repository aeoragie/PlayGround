-- @returns: single
-- @source: table Users
-- @entity: true
CREATE PROCEDURE [nested].[NspGetUser]
    @UserId UNIQUEIDENTIFIER = NULL,
    @Email VARCHAR(255) = NULL
AS
BEGIN

    SELECT
        [UserId],
        [Email],
        [EmailConfirmed],
        [PasswordHash],
        [AuthProvider],
        [FullName],
        [NickName],
        [ProfileImageUrl],
        [PhoneNumber],
        [Birthday],
        [Gender],
        [CountryCode],
        [LanguageCode],
        [TimeZone],
        [UserStatus],
        [UserRole],
        [AgreedToTermsAt],
        [AgreedToMarketingAt],
        [AgreedToPrivacyAt],
        [FailedLoginCount],
        [LockoutEndAt],
        [LastLoginAt],
        [LastLoginIp],
        [CreatedAt],
        [UpdatedAt],
        [DeletedAt]
    FROM
        [dbo].[Users] WITH (NOLOCK)
    WHERE
        ((@UserId IS NOT NULL AND [UserId] = @UserId) OR (@Email IS NOT NULL AND [Email] = @Email))
        AND [DeletedAt] IS NULL;

END
