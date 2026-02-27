-- @returns: none
CREATE PROCEDURE [dbo].[UspSaveOnboardingProfile]
    @UserId UNIQUEIDENTIFIER,
    @NickName NVARCHAR(50) = NULL,
    @SportType VARCHAR(20) = NULL,
    @AgeGroup VARCHAR(10) = NULL,
    @Region NVARCHAR(20) = NULL
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
            [NickName] = ISNULL(@NickName, [NickName]),
            [SportType] = ISNULL(@SportType, [SportType]),
            [AgeGroup] = ISNULL(@AgeGroup, [AgeGroup]),
            [Region] = ISNULL(@Region, [Region]),
            [UpdatedAt] = GETUTCDATE()
        WHERE [UserId] = @UserId
            AND [DeletedAt] IS NULL;

        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
