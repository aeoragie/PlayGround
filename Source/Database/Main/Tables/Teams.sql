-- @entity: TeamsEntity
CREATE TABLE [dbo].[Teams]
(
    [TeamId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TeamName] NVARCHAR(100) NOT NULL,
    [ShortName] NVARCHAR(20) NULL,
    [Region] NVARCHAR(100) NULL,
    [City] NVARCHAR(100) NULL,
    [AgeGroup] VARCHAR(20) NULL,            -- 'U12', 'U15', 'U17', 'U20', 'Senior'
    [LeagueName] NVARCHAR(100) NULL,
    [LogoUrl] VARCHAR(2048) NULL,
    [FoundedYear] INT NULL,
    [ManagerName] NVARCHAR(100) NULL,
    [ContactEmail] VARCHAR(255) NULL,
    [Description] NVARCHAR(1000) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,

    CONSTRAINT [PK_Teams] PRIMARY KEY ([TeamId])
);
