-- @returns: none
CREATE PROCEDURE [dbo].[UspUpdateLastLogin]
    @UserId UNIQUEIDENTIFIER,
    @IpAddress VARCHAR(45) = NULL,
    @UserAgent VARCHAR(500) = NULL,
    @DeviceType VARCHAR(50) = NULL,
    @RefreshTokenId UNIQUEIDENTIFIER = NULL,
    @SessionExpiresAt DATETIME2 = NULL
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = GETUTCDATE();

    BEGIN TRY

        BEGIN TRANSACTION;

        -- 로그인 정보 업데이트 및 실패 카운트 초기화
        UPDATE [dbo].[Users]
        SET
            [LastLoginAt] = @Now,
            [LastLoginIp] = @IpAddress,
            [FailedLoginCount] = 0,
            [LockoutEndAt] = NULL,
            [UpdatedAt] = @Now
        WHERE
            [UserId] = @UserId;

        -- 기존 활성 세션 비활성화
        UPDATE [dbo].[UserSessions]
        SET
            [IsActive] = 0
        WHERE
            [UserId] = @UserId
            AND [IsActive] = 1;

        -- 새 세션 생성
        IF @SessionExpiresAt IS NOT NULL
        BEGIN
            INSERT INTO [dbo].[UserSessions]
            (
                [UserId],
                [RefreshTokenId],
                [IpAddress],
                [UserAgent],
                [DeviceType],
                [IsActive],
                [CreatedAt],
                [LastActivityAt],
                [ExpiresAt]
            )
            VALUES
            (
                @UserId,
                @RefreshTokenId,
                @IpAddress,
                @UserAgent,
                @DeviceType,
                1,
                @Now,
                @Now,
                @SessionExpiresAt
            );
        END

        -- 감사 로그 기록
        INSERT INTO [dbo].[AuditLogs]
        (
            [UserId],
            [Action],
            [TableName],
            [EntityId],
            [PerformedAt]
        )
        VALUES
        (
            @UserId,
            'LOGIN',
            'Users',
            CAST(@UserId AS VARCHAR(255)),
            @Now
        );

        -- 로그인 시도 로그 기록
        INSERT INTO [dbo].[LoginAttemptLogs]
        (
            [Email],
            [IpAddress],
            [UserAgent],
            [IsSuccess],
            [AttemptedAt]
        )
        SELECT
            [Email],
            @IpAddress,
            @UserAgent,
            1,
            @Now
        FROM [dbo].[Users]
        WHERE [UserId] = @UserId;

        COMMIT TRANSACTION;

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
