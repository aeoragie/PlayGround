-- @returns: single
-- @source: procedure NspGetUser
CREATE PROCEDURE [dbo].[UspUpdateUser]
    @UserId UNIQUEIDENTIFIER,
    @Email VARCHAR(255) = NULL,
    @PasswordHash VARCHAR(255) = NULL,
    @FullName VARCHAR(100) = NULL,
    @NickName VARCHAR(50) = NULL,
    @ProfileImageUrl VARCHAR(2048) = NULL,
    @PhoneNumber VARCHAR(50) = NULL,
    @Birthday DATE = NULL,
    @Gender TINYINT = NULL,
    @CountryCode CHAR(2) = NULL,
    @LanguageCode VARCHAR(10) = NULL,
    @TimeZone VARCHAR(50) = NULL,
    @UserStatus VARCHAR(20) = NULL,
    @UserRole VARCHAR(20) = NULL,
    @AgreedToTermsAt DATETIME2 = NULL,
    @AgreedToMarketingAt DATETIME2 = NULL,
    @AgreedToPrivacyAt DATETIME2 = NULL
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [UserId] = @UserId AND [DeletedAt] IS NULL)
        BEGIN
            RAISERROR('User not found', 16, 1);
            RETURN -1;
        END

        UPDATE [dbo].[Users]
        SET
            [Email] = ISNULL(@Email, [Email]),
            [PasswordHash] = ISNULL(@PasswordHash, [PasswordHash]),
            [FullName] = ISNULL(@FullName, [FullName]),
            [NickName] = ISNULL(@NickName, [NickName]),
            [ProfileImageUrl] = ISNULL(@ProfileImageUrl, [ProfileImageUrl]),
            [PhoneNumber] = ISNULL(@PhoneNumber, [PhoneNumber]),
            [Birthday] = ISNULL(@Birthday, [Birthday]),
            [Gender] = ISNULL(@Gender, [Gender]),
            [CountryCode] = ISNULL(@CountryCode, [CountryCode]),
            [LanguageCode] = ISNULL(@LanguageCode, [LanguageCode]),
            [TimeZone] = ISNULL(@TimeZone, [TimeZone]),
            [UserStatus] = ISNULL(@UserStatus, [UserStatus]),
            [UserRole] = ISNULL(@UserRole, [UserRole]),
            [AgreedToTermsAt] = ISNULL(@AgreedToTermsAt, [AgreedToTermsAt]),
            [AgreedToMarketingAt] = ISNULL(@AgreedToMarketingAt, [AgreedToMarketingAt]),
            [AgreedToPrivacyAt] = ISNULL(@AgreedToPrivacyAt, [AgreedToPrivacyAt]),
            [UpdatedAt] = GETUTCDATE()
        WHERE [UserId] = @UserId;

        -- Results: Procedure:NspGetUser
        EXEC [nested].[NspGetUser] @UserId = @UserId;
        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
