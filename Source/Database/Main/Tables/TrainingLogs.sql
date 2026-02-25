-- @entity: TrainingLogsEntity
CREATE TABLE [dbo].[TrainingLogs]
(
    [TrainingLogId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [TrainingType] VARCHAR(20) NOT NULL,     -- 'Team','Individual','Physical','Tactical'
    [TrainingDate] DATE NOT NULL,
    [Intensity] INT NOT NULL,                -- 1~10
    [DurationMinutes] INT NOT NULL,
    [Notes] NVARCHAR(500) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_TrainingLogs] PRIMARY KEY ([TrainingLogId]),
    CONSTRAINT [FK_TrainingLogs_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [CK_TrainingLogs_Intensity] CHECK ([Intensity] >= 1 AND [Intensity] <= 10)
);
