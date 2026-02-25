-- @returns: list
-- @source: table Users
CREATE PROCEDURE [dbo].[UspGetUserList]
    @UserStatus VARCHAR(20) = NULL,
    @UserRole VARCHAR(20) = NULL,
    @EmailConfirmed BIT = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 100
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY

        IF @PageNumber < 1 SET @PageNumber = 1;
        IF @PageSize < 1 SET @PageSize = 100;
        IF @PageSize > 1000 SET @PageSize = 1000;

        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        -- ResultType: Table:Users
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
            [UpdatedAt]
        FROM
            [dbo].[Users] WITH (NOLOCK)
        WHERE
            (@UserStatus IS NULL OR [UserStatus] = @UserStatus)
            AND (@UserRole IS NULL OR [UserRole] = @UserRole)
            AND (@EmailConfirmed IS NULL OR [EmailConfirmed] = @EmailConfirmed)
            AND [DeletedAt] IS NULL
        ORDER BY [CreatedAt] DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
