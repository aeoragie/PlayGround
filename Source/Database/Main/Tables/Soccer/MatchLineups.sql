-- @entity: SoccerMatchLineupsEntity
CREATE TABLE [Soccer].[MatchLineups]
(
    [MatchLineupId]  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [MatchId]        UNIQUEIDENTIFIER NOT NULL,  -- FK → dbo.Matches

    [PlayerName]     NVARCHAR(50) NOT NULL,
    [Position]       VARCHAR(5) NOT NULL,        -- GK, DF, MF, FW
    [JerseyNumber]   INT NOT NULL,
    [PlayTime]       INT NULL,                   -- 출전 시간 (분)
    [Status]         CHAR(1) NOT NULL,           -- 'S'=선발, 'R'=교체
    [Side]           CHAR(1) NOT NULL,           -- 'H'=홈, 'A'=원정
    [IsCaptain]      BIT NOT NULL DEFAULT 0,

    [GoalTime]       VARCHAR(50) NULL,           -- 득점 시간 (복수 가능 "15,42")
    [YellowTime]     VARCHAR(50) NULL,
    [RedTime]        VARCHAR(50) NULL,
    [AssistTime]     VARCHAR(50) NULL,

    [CreatedAt]      DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Soccer_MatchLineups] PRIMARY KEY ([MatchLineupId]),
    CONSTRAINT [FK_Soccer_MatchLineups_Matches] FOREIGN KEY ([MatchId])
        REFERENCES [dbo].[Matches]([MatchId]),
    CONSTRAINT [CK_Soccer_MatchLineups_Status] CHECK ([Status] IN ('S', 'R')),
    CONSTRAINT [CK_Soccer_MatchLineups_Side] CHECK ([Side] IN ('H', 'A'))
);
