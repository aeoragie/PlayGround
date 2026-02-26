-- @entity: SoccerMatchDetailsEntity
CREATE TABLE [Soccer].[MatchDetails]
(
    [MatchId]              UNIQUEIDENTIFIER NOT NULL,  -- PK이자 FK (1:1 with dbo.Matches)

    [HomeScoreFirstHalf]   INT NULL,
    [AwayScoreFirstHalf]   INT NULL,
    [HomeScoreSecondHalf]  INT NULL,
    [AwayScoreSecondHalf]  INT NULL,

    [RefereeMain]          NVARCHAR(50) NULL,
    [Weather]              NVARCHAR(20) NULL,
    [Attendance]           INT NULL,
    [PlayingTime]          INT NULL,                   -- 경기 시간 (분)
    [TotalTimeDesc]        NVARCHAR(100) NULL,         -- "40분 / 전후반 20분+20분"

    [HomeCoach]            NVARCHAR(50) NULL,
    [AwayCoach]            NVARCHAR(50) NULL,

    [HomeYellowCards]      INT NOT NULL DEFAULT 0,
    [AwayYellowCards]      INT NOT NULL DEFAULT 0,
    [HomeRedCards]         INT NOT NULL DEFAULT 0,
    [AwayRedCards]         INT NOT NULL DEFAULT 0,

    [CreatedAt]            DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Soccer_MatchDetails] PRIMARY KEY ([MatchId]),
    CONSTRAINT [FK_Soccer_MatchDetails_Matches] FOREIGN KEY ([MatchId])
        REFERENCES [dbo].[Matches]([MatchId])
);
