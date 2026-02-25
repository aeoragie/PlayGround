-- @entity: PhysicalRecordsEntity
CREATE TABLE [dbo].[PhysicalRecords]
(
    [PhysicalRecordId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [TestType] VARCHAR(30) NOT NULL,         -- 'Sprint10m','Sprint30m','VerticalJump','Agility','Endurance' 등
    [Value] DECIMAL(10,2) NOT NULL,          -- 측정값
    [Unit] VARCHAR(10) NOT NULL,             -- 'sec','cm','m','%','level'
    [MeasuredAt] DATE NOT NULL,
    [MeasuredBy] UNIQUEIDENTIFIER NULL,      -- 측정자 UserId

    [Notes] NVARCHAR(300) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PhysicalRecords] PRIMARY KEY ([PhysicalRecordId]),
    CONSTRAINT [FK_PhysicalRecords_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId])
);
