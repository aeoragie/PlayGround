CREATE PROCEDURE [nested].[NspException]
AS
BEGIN

	IF @@TRANCOUNT > 0
	BEGIN
        ROLLBACK TRAN;
	END

	DECLARE @ErrorMessage varchar(1000);  
    DECLARE @ErrorSeverity int;
    DECLARE @ErrorState int;
    DECLARE @ErrorProcedure varchar(1000);
    DECLARE @ErrorLine int;
    DECLARE @ErrorNumber int;

    SELECT  @ErrorNumber = ERROR_NUMBER(), 
            @ErrorSeverity = ERROR_SEVERITY(), 
            @ErrorState = ERROR_STATE(), 
            @ErrorProcedure = ERROR_PROCEDURE(), 
            @ErrorLine = ERROR_LINE(), 
            @ErrorMessage = ERROR_MESSAGE();

    INSERT INTO
        [nested].[Exceptions]
    VALUES
        (@ErrorNumber, @ErrorSeverity, @ErrorState, @ErrorProcedure, @ErrorLine, @ErrorMessage, GETUTCDATE());    

END
