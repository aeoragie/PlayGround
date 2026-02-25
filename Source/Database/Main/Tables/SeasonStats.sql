-- @entity: SeasonStatsEntity
CREATE TABLE [dbo].[SeasonStats]
(
    [SeasonStatId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,
    [CompetitionId] UNIQUEIDENTIFIER NULL,

    [Season] VARCHAR(20) NOT NULL,           -- '2025-2026'
    [CompetitionName] NVARCHAR(200) NULL,    -- 대회명 보존
    [Position] VARCHAR(5) NOT NULL,

    -- 공통 기록
    [Appearances] INT NOT NULL DEFAULT 0,
    [StartingLineup] INT NOT NULL DEFAULT 0,
    [MinutesPlayed] INT NOT NULL DEFAULT 0,
    [Goals] INT NOT NULL DEFAULT 0,
    [Assists] INT NOT NULL DEFAULT 0,
    [YellowCards] INT NOT NULL DEFAULT 0,
    [RedCards] INT NOT NULL DEFAULT 0,

    -- 공격 (FW/MF)
    [Shots] INT NULL,
    [ShotsOnTarget] INT NULL,

    -- 패스 (MF)
    [Passes] INT NULL,
    [PassAccuracy] DECIMAL(5,2) NULL,        -- %
    [KeyPasses] INT NULL,

    -- 수비 (DF)
    [Tackles] INT NULL,
    [Interceptions] INT NULL,
    [Clearances] INT NULL,

    -- 골키퍼 (GK)
    [Saves] INT NULL,
    [GoalsConceded] INT NULL,
    [CleanSheets] INT NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_SeasonStats] PRIMARY KEY ([SeasonStatId]),
    CONSTRAINT [FK_SeasonStats_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_SeasonStats_Competitions] FOREIGN KEY ([CompetitionId]) REFERENCES [dbo].[Competitions]([CompetitionId])
);
