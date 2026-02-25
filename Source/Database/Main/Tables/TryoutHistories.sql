-- @entity: TryoutHistoriesEntity
-- 트라이아웃 지원 이력
CREATE TABLE [dbo].[TryoutHistories]
(
    [TryoutHistoryId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,
    [TeamId] UNIQUEIDENTIFIER NULL,

    [TeamName] NVARCHAR(100) NOT NULL,
    [TryoutDate] DATE NOT NULL,
    [Result] VARCHAR(20) NULL,               -- 'Passed','Failed','Pending','Withdrawn'
    [Notes] NVARCHAR(500) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_TryoutHistories] PRIMARY KEY ([TryoutHistoryId]),
    CONSTRAINT [FK_TryoutHistories_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_TryoutHistories_Teams] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams]([TeamId])
);
