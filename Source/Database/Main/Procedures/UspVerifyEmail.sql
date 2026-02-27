-- @returns: none
CREATE PROCEDURE [dbo].[UspVerifyEmail]
    @VerificationId UNIQUEIDENTIFIER,
    @VerificationCode VARCHAR(10)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @Email VARCHAR(255);
    DECLARE @IsValid BIT = 0;

    BEGIN TRY

        -- 인증 코드 검증
        SELECT @Email = [Email], @IsValid = 1
        FROM [dbo].[EmailVerifications]
        WHERE [VerificationId] = @VerificationId
            AND [VerificationCode] = @VerificationCode
            AND [IsVerified] = 0
            AND [ExpiresAt] > GETUTCDATE()
            AND [AttemptCount] < 5;

        IF @IsValid = 0
        BEGIN
            -- 시도 횟수 증가
            UPDATE [dbo].[EmailVerifications]
            SET [AttemptCount] = [AttemptCount] + 1
            WHERE [VerificationId] = @VerificationId
                AND [IsVerified] = 0;

            RAISERROR('Invalid or expired verification code', 16, 1);
            RETURN -1;
        END

        -- 인증 완료 처리
        UPDATE [dbo].[EmailVerifications]
        SET [IsVerified] = 1,
            [VerifiedAt] = GETUTCDATE()
        WHERE [VerificationId] = @VerificationId;

        -- 사용자 이메일 인증 상태 업데이트
        UPDATE [dbo].[Users]
        SET [EmailConfirmed] = 1,
            [UpdatedAt] = GETUTCDATE()
        WHERE [Email] = @Email
            AND [DeletedAt] IS NULL;

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
