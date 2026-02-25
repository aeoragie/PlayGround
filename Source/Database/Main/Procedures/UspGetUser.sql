-- @returns: single
-- @source: procedure NspGetUser
CREATE PROCEDURE [dbo].[UspGetUser]
    @UserId UNIQUEIDENTIFIER = NULL,
    @Email VARCHAR(255) = NULL
AS
BEGIN

    SET NOCOUNT ON;
    
    BEGIN TRY

        -- Results: Procedure:NspGetUser
        EXEC [nested].[NspGetUser] @UserId, @Email;
        RETURN 0;
            
    END TRY
    BEGIN CATCH

        EXEC [nested].[NspException];
        RETURN -1;

    END CATCH

END