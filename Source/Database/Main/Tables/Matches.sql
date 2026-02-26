-- @entity: MatchesEntity
CREATE TABLE [dbo].[Matches]
(
    [MatchId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CompetitionId] UNIQUEIDENTIFIER NULL,
    [SportType] VARCHAR(20) NOT NULL DEFAULT 'Soccer',  -- 'Soccer', 'Basketball', 'Baseball'

    [HomeTeamId] UNIQUEIDENTIFIER NOT NULL,
    [AwayTeamId] UNIQUEIDENTIFIER NOT NULL,
    [HomeTeamName] NVARCHAR(100) NOT NULL,
    [AwayTeamName] NVARCHAR(100) NOT NULL,

    [MatchDate] DATETIME2 NOT NULL,
    [Venue] NVARCHAR(200) NULL,
    [Season] VARCHAR(20) NULL,
    [MatchNumber] INT NULL,                  -- 대회 내 경기 번호
    [GroupName] NVARCHAR(20) NULL,           -- 조별 리그 조명 (A, B 등)

    [HomeScore] INT NULL,
    [AwayScore] INT NULL,
    [HomePkScore] INT NULL,                  -- 승부차기 홈 점수
    [AwayPkScore] INT NULL,                  -- 승부차기 원정 점수
    [MatchStatus] VARCHAR(20) NOT NULL DEFAULT 'Scheduled', -- 'Scheduled','InProgress','Completed','Cancelled'
    [ExternalId] VARCHAR(64) NULL,           -- 외부 시스템 ID (KFA SingleIdx 등)

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Matches] PRIMARY KEY ([MatchId]),
    CONSTRAINT [FK_Matches_Competitions] FOREIGN KEY ([CompetitionId]) REFERENCES [dbo].[Competitions]([CompetitionId]),
    CONSTRAINT [FK_Matches_HomeTeam] FOREIGN KEY ([HomeTeamId]) REFERENCES [dbo].[Teams]([TeamId]),
    CONSTRAINT [FK_Matches_AwayTeam] FOREIGN KEY ([AwayTeamId]) REFERENCES [dbo].[Teams]([TeamId])
);
