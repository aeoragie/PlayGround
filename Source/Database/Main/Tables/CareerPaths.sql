-- @entity: CareerPathsEntity
-- 희망 진로 관리
CREATE TABLE [dbo].[CareerPaths]
(
    [CareerPathId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [CareerGoalType] VARCHAR(30) NOT NULL,   -- 'Professional','SportsSchool','Academic','Other'
    [Description] NVARCHAR(500) NULL,
    [Visibility] VARCHAR(20) NOT NULL DEFAULT 'Private', -- 'Public','TeamOnly','Private','RequestOnly'

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_CareerPaths] PRIMARY KEY ([CareerPathId]),
    CONSTRAINT [FK_CareerPaths_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId])
);
