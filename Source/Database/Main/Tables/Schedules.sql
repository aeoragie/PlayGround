-- @entity: SchedulesEntity
CREATE TABLE [dbo].[Schedules]
(
    [ScheduleId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NULL,        -- 개인 일정
    [TeamId] UNIQUEIDENTIFIER NULL,          -- 팀 일정 (팀원 자동 동기화)

    [ScheduleType] VARCHAR(20) NOT NULL,     -- 'Match','Training','Test','Meeting','Tryout'
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Location] NVARCHAR(200) NULL,

    [StartAt] DATETIME2 NOT NULL,
    [EndAt] DATETIME2 NULL,
    [IsAllDay] BIT NOT NULL DEFAULT 0,

    -- 연결 (해당되는 경우)
    [MatchId] UNIQUEIDENTIFIER NULL,

    -- 알림
    [ReminderDays] INT NULL,                 -- D-N 알림 (1, 3 등)

    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Schedules] PRIMARY KEY ([ScheduleId]),
    CONSTRAINT [FK_Schedules_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_Schedules_Teams] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams]([TeamId]),
    CONSTRAINT [FK_Schedules_Matches] FOREIGN KEY ([MatchId]) REFERENCES [dbo].[Matches]([MatchId]),
    CONSTRAINT [FK_Schedules_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([UserId])
);
