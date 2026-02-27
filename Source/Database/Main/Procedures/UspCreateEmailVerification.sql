-- @returns: single
-- @source: custom
-- @column: VerificationId uniqueidentifier NOT NULL
-- @column: VerificationCode varchar(10) NOT NULL
-- @column: ExpiresAt datetime2 NOT NULL
CREATE PROCEDURE [dbo].[UspCreateEmailVerification]
    @Email VARCHAR(255),
    @Purpose VARCHAR(20) = 'SignUp',
    @RequestIp VARCHAR(45) = NULL
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @VerificationId UNIQUEIDENTIFIER = NEWID();
    DECLARE @Code VARCHAR(10);
    DECLARE @ExpiresAt DATETIME2 = DATEADD(MINUTE, 10, GETUTCDATE());

    BEGIN TRY

        -- 6자리 숫자 코드 생성
        SET @Code = RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS VARCHAR(6)), 6);

        -- 이전 미인증 코드 무효화
        UPDATE [dbo].[EmailVerifications]
        SET [ExpiresAt] = GETUTCDATE()
        WHERE [Email] = @Email
            AND [Purpose] = @Purpose
            AND [IsVerified] = 0
            AND [ExpiresAt] > GETUTCDATE();

        -- 새 인증 코드 생성
        INSERT INTO [dbo].[EmailVerifications]
        (
            [VerificationId],
            [Email],
            [VerificationCode],
            [Purpose],
            [RequestIp],
            [AttemptCount],
            [IsVerified],
            [ExpiresAt],
            [CreatedAt]
        )
        VALUES
        (
            @VerificationId,
            @Email,
            @Code,
            @Purpose,
            @RequestIp,
            0,
            0,
            @ExpiresAt,
            GETUTCDATE()
        );

        SELECT @VerificationId AS [VerificationId],
               @Code AS [VerificationCode],
               @ExpiresAt AS [ExpiresAt];

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
