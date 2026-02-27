-- @returns: none
CREATE PROCEDURE [dbo].[UspResetPassword]
    @TokenHash VARCHAR(255),
    @NewPasswordHash VARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @UserId UNIQUEIDENTIFIER;

    BEGIN TRY

        -- 토큰 검증
        SELECT @UserId = [UserId]
        FROM [dbo].[PasswordResetTokens]
        WHERE [TokenHash] = @TokenHash
            AND [UsedAt] IS NULL
            AND [ExpiresAt] > GETUTCDATE();

        IF @UserId IS NULL
        BEGIN
            RAISERROR('Invalid or expired reset token', 16, 1);
            RETURN -1;
        END

        -- 토큰 사용 처리
        UPDATE [dbo].[PasswordResetTokens]
        SET [UsedAt] = GETUTCDATE()
        WHERE [TokenHash] = @TokenHash
            AND [UsedAt] IS NULL;

        -- 비밀번호 변경
        UPDATE [dbo].[Users]
        SET [PasswordHash] = @NewPasswordHash,
            [UpdatedAt] = GETUTCDATE()
        WHERE [UserId] = @UserId
            AND [DeletedAt] IS NULL;

        -- 모든 RefreshToken 폐기 (보안)
        UPDATE [dbo].[UserRefreshTokens]
        SET [IsRevoked] = 1,
            [RevokedAt] = GETUTCDATE(),
            [RevokedReason] = 'PasswordReset'
        WHERE [UserId] = @UserId
            AND [IsRevoked] = 0;

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
