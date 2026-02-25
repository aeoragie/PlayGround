-- @returns: single
-- @source: procedure NspGetUser
CREATE PROCEDURE [dbo].[UspCreateUserByEmail]
    @Email VARCHAR(255),
    @PasswordHash VARCHAR(255),
    @FullName VARCHAR(100),
    @NickName VARCHAR(50) = NULL,
    @UserRole VARCHAR(20) = 'Player',
    @AgreedToTermsAt DATETIME2 = NULL,
    @AgreedToMarketingAt DATETIME2 = NULL,
    @AgreedToPrivacyAt DATETIME2 = NULL,
    @CountryCode CHAR(2) = NULL,
    @LanguageCode VARCHAR(10) = 'ko',
    @TimeZone VARCHAR(50) = 'Korea Standard Time'
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @UserId UNIQUEIDENTIFIER = NEWID();

    BEGIN TRY

        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Users]
        (
            [UserId],
            [Email],
            [EmailConfirmed],
            [PasswordHash],
            [AuthProvider],
            [FullName],
            [NickName],
            [UserRole],
            [AgreedToTermsAt],
            [AgreedToMarketingAt],
            [AgreedToPrivacyAt],
            [CountryCode],
            [LanguageCode],
            [TimeZone],
            [UserStatus],
            [FailedLoginCount],
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES
        (
            @UserId,
            @Email,
            0,
            @PasswordHash,
            'Local',
            @FullName,
            @NickName,
            @UserRole,
            @AgreedToTermsAt,
            @AgreedToMarketingAt,
            @AgreedToPrivacyAt,
            @CountryCode,
            @LanguageCode,
            @TimeZone,
            'Active',
            0,
            GETUTCDATE(),
            GETUTCDATE()
        );

        COMMIT TRANSACTION;

        -- Results: Procedure:NspGetUser
        EXEC [nested].[NspGetUser] @UserId = @UserId;
        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
