-- @entity: PlayerGoalsEntity
CREATE TABLE [dbo].[PlayerGoals]
(
    [PlayerGoalId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [GoalType] VARCHAR(20) NOT NULL,         -- 'Technical','Physical','Performance','Career'
    [Title] NVARCHAR(200) NOT NULL,          -- '올해 5득점 달성', 'U-17 대표팀 선발'
    [Description] NVARCHAR(500) NULL,
    [TargetValue] DECIMAL(10,2) NULL,        -- 목표 수치
    [CurrentValue] DECIMAL(10,2) NULL,       -- 현재 달성 수치
    [Deadline] DATE NULL,
    [Season] VARCHAR(20) NULL,

    [CoachId] UNIQUEIDENTIFIER NULL,         -- 지도자와 공동 설정 시
    [Status] VARCHAR(20) NOT NULL DEFAULT 'NotStarted', -- 'NotStarted','InProgress','Completed','Failed','Cancelled'

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PlayerGoals] PRIMARY KEY ([PlayerGoalId]),
    CONSTRAINT [FK_PlayerGoals_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId])
);
