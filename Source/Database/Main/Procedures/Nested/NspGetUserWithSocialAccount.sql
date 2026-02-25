-- @returns: single
-- @source: join
-- @join: Users AS u (UserId, Email, EmailConfirmed, FullName, NickName, ProfileImageUrl, PhoneNumber, Birthday, Gender, CountryCode, LanguageCode, TimeZone, UserStatus, UserRole, AgreedToTermsAt, AgreedToMarketingAt, AgreedToPrivacyAt, FailedLoginCount, LockoutEndAt, LastLoginAt, LastLoginIp, CreatedAt, UpdatedAt)
-- @join: UserSocialAccounts AS sa (SocialAccountId, Provider, ProviderUserId)
-- @entity: UserWithSocialAccountEntity
CREATE PROCEDURE [nested].[NspGetUserWithSocialAccount]
    @Provider VARCHAR(50),
    @ProviderUserId VARCHAR(255)
AS
BEGIN

    -- Results: Users u, UserSocialAccounts sa
    SELECT
        u.[UserId],
        u.[Email],
        u.[EmailConfirmed],
        u.[FullName],
        u.[NickName],
        u.[ProfileImageUrl],
        u.[PhoneNumber],
        u.[Birthday],
        u.[Gender],
        u.[CountryCode],
        u.[LanguageCode],
        u.[TimeZone],
        u.[UserStatus],
        u.[UserRole],
        u.[AgreedToTermsAt],
        u.[AgreedToMarketingAt],
        u.[AgreedToPrivacyAt],
        u.[FailedLoginCount],
        u.[LockoutEndAt],
        u.[LastLoginAt],
        u.[LastLoginIp],
        u.[CreatedAt],
        u.[UpdatedAt],
        sa.[SocialAccountId],
        sa.[Provider],
        sa.[ProviderUserId]
    FROM
        [dbo].[Users] u WITH(NOLOCK)
        INNER JOIN [dbo].[UserSocialAccounts] sa WITH(NOLOCK) ON u.[UserId] = sa.[UserId]
    WHERE
        sa.[Provider] = @Provider
        AND sa.[ProviderUserId] = @ProviderUserId
        AND u.[DeletedAt] IS NULL;

END
