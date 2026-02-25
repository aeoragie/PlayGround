-- @returns: single
-- @source: custom
-- @column: TokenId uniqueidentifier NOT NULL
CREATE PROCEDURE [dbo].[UspCreateRefreshToken]
    @UserId UNIQUEIDENTIFIER,
    @Token VARCHAR(500),
    @DeviceInfo VARCHAR(255) = NULL,
    @IpAddress VARCHAR(45) = NULL,
    @ExpiresAt DATETIME2
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY

        DECLARE @Now DATETIME2 = GETUTCDATE();
        DECLARE @TokenId UNIQUEIDENTIFIER = NEWID();

        BEGIN TRANSACTION;

        -- 기존 유효 토큰 무효화
        UPDATE [dbo].[UserRefreshTokens]
        SET
            [IsRevoked] = 1,
            [RevokedAt] = @Now,
            [RevokedReason] = 'NewTokenIssued'
        WHERE
            [UserId] = @UserId
            AND [IsRevoked] = 0
            AND [ExpiresAt] > @Now;

        INSERT INTO [dbo].[UserRefreshTokens]
        (
            [TokenId],
            [UserId],
            [Token],
            [DeviceInfo],
            [IpAddress],
            [IsRevoked],
            [ExpiresAt],
            [CreatedAt]
        )
        VALUES
        (
            @TokenId,
            @UserId,
            @Token,
            @DeviceInfo,
            @IpAddress,
            0,
            @ExpiresAt,
            @Now
        );

        COMMIT TRANSACTION;

        -- Results: Custom
        SELECT @TokenId AS TokenId;

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
