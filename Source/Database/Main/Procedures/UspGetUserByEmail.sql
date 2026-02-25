-- @returns: single
-- @source: procedure NspGetUser
CREATE PROCEDURE [dbo].[UspGetUserByEmail]
    @Email VARCHAR(255)
AS
BEGIN

    SET NOCOUNT ON;
    
    BEGIN TRY

        -- Results: Procedure:NspGetUser
        EXEC [nested].[NspGetUser] NULL, @Email;
        RETURN 0;

    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END