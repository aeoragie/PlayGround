-- @returns: single
-- @source: custom
-- @column: TokenId uniqueidentifier NOT NULL
CREATE PROCEDURE [dbo].[UspCreatePasswordResetToken]
    @Email VARCHAR(255),
    @TokenHash VARCHAR(255),
    @RequestIp VARCHAR(45) = NULL
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @TokenId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ExpiresAt DATETIME2 = DATEADD(HOUR, 1, GETUTCDATE());

    BEGIN TRY

        -- 사용자 조회
        SELECT @UserId = [UserId]
        FROM [dbo].[Users]
        WHERE [Email] = @Email
            AND [DeletedAt] IS NULL;

        IF @UserId IS NULL
        BEGIN
            -- 보안: 사용자 존재 여부 노출 방지 (성공처럼 응답)
            SELECT NEWID() AS [TokenId];
            RETURN 0;
        END

        -- 기존 미사용 토큰 무효화
        UPDATE [dbo].[PasswordResetTokens]
        SET [ExpiresAt] = GETUTCDATE()
        WHERE [UserId] = @UserId
            AND [UsedAt] IS NULL
            AND [ExpiresAt] > GETUTCDATE();

        -- 새 토큰 생성
        INSERT INTO [dbo].[PasswordResetTokens]
        (
            [TokenId],
            [UserId],
            [TokenHash],
            [RequestIp],
            [ExpiresAt],
            [CreatedAt]
        )
        VALUES
        (
            @TokenId,
            @UserId,
            @TokenHash,
            @RequestIp,
            @ExpiresAt,
            GETUTCDATE()
        );

        SELECT @TokenId AS [TokenId];

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
