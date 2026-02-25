-- @entity: CareerEntriesEntity
CREATE TABLE [dbo].[CareerEntries]
(
    [CareerEntryId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,
    [TeamId] UNIQUEIDENTIFIER NULL,

    [TeamName] NVARCHAR(100) NOT NULL,       -- 팀 삭제 시에도 이력 보존
    [LeagueName] NVARCHAR(100) NULL,
    [Role] NVARCHAR(50) NULL,                -- 주요 역할/포지션
    [CoachName] NVARCHAR(100) NULL,          -- 당시 감독/코치명
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NULL,                     -- NULL = 현재 소속

    -- 대표팀 여부
    [IsNationalTeam] BIT NOT NULL DEFAULT 0,
    [NationalTeamLevel] VARCHAR(10) NULL,    -- 'U12','U15','U17','U20','U23','Senior'

    [Description] NVARCHAR(500) NULL,        -- 성장 스토리
    [SortOrder] INT NOT NULL DEFAULT 0,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_CareerEntries] PRIMARY KEY ([CareerEntryId]),
    CONSTRAINT [FK_CareerEntries_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_CareerEntries_Teams] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams]([TeamId])
);
