-- @entity: PlayerMatchStatsEntity
CREATE TABLE [dbo].[PlayerMatchStats]
(
    [PlayerMatchStatId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,
    [MatchId] UNIQUEIDENTIFIER NOT NULL,

    [MinutesPlayed] INT NOT NULL DEFAULT 0,
    [Goals] INT NOT NULL DEFAULT 0,
    [Assists] INT NOT NULL DEFAULT 0,
    [YellowCards] INT NOT NULL DEFAULT 0,
    [RedCards] INT NOT NULL DEFAULT 0,
    [Rating] DECIMAL(3,1) NULL,              -- 1.0~10.0 경기 평점

    [IsStarting] BIT NOT NULL DEFAULT 0,
    [SubstitutedIn] INT NULL,                -- 교체 투입 시간 (분)
    [SubstitutedOut] INT NULL,               -- 교체 아웃 시간 (분)

    [Notes] NVARCHAR(500) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PlayerMatchStats] PRIMARY KEY ([PlayerMatchStatId]),
    CONSTRAINT [FK_PlayerMatchStats_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_PlayerMatchStats_Matches] FOREIGN KEY ([MatchId]) REFERENCES [dbo].[Matches]([MatchId]),
    CONSTRAINT [UQ_PlayerMatchStats_PlayerMatch] UNIQUE ([PlayerId], [MatchId])
);
