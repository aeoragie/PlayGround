-- @entity: MatchesEntity
CREATE TABLE [dbo].[Matches]
(
    [MatchId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CompetitionId] UNIQUEIDENTIFIER NULL,

    [HomeTeamId] UNIQUEIDENTIFIER NOT NULL,
    [AwayTeamId] UNIQUEIDENTIFIER NOT NULL,
    [HomeTeamName] NVARCHAR(100) NOT NULL,
    [AwayTeamName] NVARCHAR(100) NOT NULL,

    [MatchDate] DATETIME2 NOT NULL,
    [Venue] NVARCHAR(200) NULL,
    [Season] VARCHAR(20) NULL,

    [HomeScore] INT NULL,
    [AwayScore] INT NULL,
    [MatchStatus] VARCHAR(20) NOT NULL DEFAULT 'Scheduled', -- 'Scheduled','InProgress','Completed','Cancelled'

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Matches] PRIMARY KEY ([MatchId]),
    CONSTRAINT [FK_Matches_Competitions] FOREIGN KEY ([CompetitionId]) REFERENCES [dbo].[Competitions]([CompetitionId]),
    CONSTRAINT [FK_Matches_HomeTeam] FOREIGN KEY ([HomeTeamId]) REFERENCES [dbo].[Teams]([TeamId]),
    CONSTRAINT [FK_Matches_AwayTeam] FOREIGN KEY ([AwayTeamId]) REFERENCES [dbo].[Teams]([TeamId])
);
