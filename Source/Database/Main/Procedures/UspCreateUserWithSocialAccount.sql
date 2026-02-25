-- @returns: single
-- @source: procedure NspGetUserWithSocialAccount
CREATE PROCEDURE [dbo].[UspCreateUserWithSocialAccount]
    @Email VARCHAR(255),
    @FullName VARCHAR(100),
    @ProfileImageUrl VARCHAR(2048) = NULL,
    @UserRole VARCHAR(20) = 'Player',
    @Provider VARCHAR(50),
    @ProviderUserId VARCHAR(255),
    @CountryCode CHAR(2) = NULL,
    @LanguageCode VARCHAR(10) = 'ko',
    @TimeZone VARCHAR(50) = 'Korea Standard Time',
    @IpAddress VARCHAR(45) = NULL,
    @UserAgent VARCHAR(500) = NULL
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @UserId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Now DATETIME2 = GETUTCDATE();

    BEGIN TRY

        BEGIN TRANSACTION;

        INSERT INTO [dbo].[Users]
        (
            [UserId],
            [Email],
            [EmailConfirmed],
            [AuthProvider],
            [FullName],
            [ProfileImageUrl],
            [CountryCode],
            [LanguageCode],
            [TimeZone],
            [UserStatus],
            [UserRole],
            [AgreedToTermsAt],
            [AgreedToPrivacyAt],
            [FailedLoginCount],
            [LastLoginAt],
            [LastLoginIp],
            [CreatedAt],
            [UpdatedAt]
        )
        VALUES
        (
            @UserId,
            @Email,
            1,
            @Provider,
            @FullName,
            @ProfileImageUrl,
            @CountryCode,
            @LanguageCode,
            @TimeZone,
            'Active',
            @UserRole,
            @Now,
            @Now,
            0,
            @Now,
            @IpAddress,
            @Now,
            @Now
        );

        INSERT INTO [dbo].[UserSocialAccounts]
        (
            [UserId],
            [Provider],
            [ProviderUserId],
            [CreatedAt]
        )
        VALUES
        (
            @UserId,
            @Provider,
            @ProviderUserId,
            @Now
        );

        INSERT INTO [dbo].[AuditLogs]
        (
            [UserId],
            [Action],
            [TableName],
            [EntityId],
            [NewValues],
            [PerformedAt]
        )
        VALUES
        (
            @UserId,
            'REGISTER',
            'Users',
            CAST(@UserId AS VARCHAR(255)),
            CONCAT('Provider: ', @Provider),
            @Now
        );

        COMMIT TRANSACTION;

        -- Results: Procedure:NspGetUserWithSocialAccount
        EXEC [nested].[NspGetUserWithSocialAccount] @Provider, @ProviderUserId;
        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
