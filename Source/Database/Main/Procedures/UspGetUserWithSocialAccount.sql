-- @returns: single
-- @source: procedure NspGetUserWithSocialAccount
CREATE PROCEDURE [dbo].[UspGetUserWithSocialAccount]
    @Provider VARCHAR(50),
    @ProviderUserId VARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;

    BEGIN TRY

        -- Results: Procedure:NspGetUserWithSocialAccount
        EXEC [nested].[NspGetUserWithSocialAccount] @Provider, @ProviderUserId;
        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END
