-- @entity: SoccerMatchEventsEntity
CREATE TABLE [Soccer].[MatchEvents]
(
    [MatchEventId]  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [MatchId]       UNIQUEIDENTIFIER NOT NULL,  -- FK → dbo.Matches

    [PlayerName]    NVARCHAR(50) NOT NULL,
    [EventCode]     INT NOT NULL,               -- 11=득점, 31=경고, 51=교체투입, 52=교체아웃
    [EventName]     NVARCHAR(20) NOT NULL,      -- '득점', '경고', '교체' 등
    [Minute]        INT NOT NULL,               -- 발생 시간 (분)
    [JerseyNumber]  INT NULL,
    [Side]          CHAR(1) NOT NULL,           -- 'H'=홈, 'A'=원정
    [IsPenalty]     BIT NOT NULL DEFAULT 0,

    [CreatedAt]     DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Soccer_MatchEvents] PRIMARY KEY ([MatchEventId]),
    CONSTRAINT [FK_Soccer_MatchEvents_Matches] FOREIGN KEY ([MatchId])
        REFERENCES [dbo].[Matches]([MatchId]),
    CONSTRAINT [CK_Soccer_MatchEvents_Side] CHECK ([Side] IN ('H', 'A'))
);
