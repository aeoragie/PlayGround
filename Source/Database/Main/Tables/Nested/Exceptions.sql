CREATE TABLE [nested].[Exceptions]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ErrorNumber] INT NOT NULL, 
    [ErrorSeverity] INT NOT NULL, 
    [ErrorState] INT NOT NULL, 
    [ErrorProcedure] VARCHAR(1000) NOT NULL, 
    [ErrorLine] INT NULL, 
    [ErrorMessage] VARCHAR(1000) NOT NULL, 
    [RegisterDate] DATETIME2 NOT NULL
)
