-- @entity: SoccerStandingsEntity
CREATE TABLE [Soccer].[Standings]
(
    [StandingId]     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CompetitionId]  UNIQUEIDENTIFIER NOT NULL,  -- FK → dbo.Competitions
    [TeamId]         UNIQUEIDENTIFIER NOT NULL,   -- FK → dbo.Teams
    [GroupName]      NVARCHAR(20) NULL,

    [Rank]           INT NOT NULL,
    [Played]         INT NOT NULL DEFAULT 0,
    [Won]            INT NOT NULL DEFAULT 0,
    [Drawn]          INT NOT NULL DEFAULT 0,
    [Lost]           INT NOT NULL DEFAULT 0,
    [GoalsFor]       INT NOT NULL DEFAULT 0,
    [GoalsAgainst]   INT NOT NULL DEFAULT 0,
    [GoalDiff]       AS ([GoalsFor] - [GoalsAgainst]),  -- Computed
    [Points]         AS ([Won] * 3 + [Drawn]),          -- Computed

    [UpdatedAt]      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Soccer_Standings] PRIMARY KEY ([StandingId]),
    CONSTRAINT [FK_Soccer_Standings_Competitions] FOREIGN KEY ([CompetitionId])
        REFERENCES [dbo].[Competitions]([CompetitionId]),
    CONSTRAINT [FK_Soccer_Standings_Teams] FOREIGN KEY ([TeamId])
        REFERENCES [dbo].[Teams]([TeamId]),
    CONSTRAINT [UQ_Soccer_Standings] UNIQUE ([CompetitionId], [TeamId], [GroupName])
);
