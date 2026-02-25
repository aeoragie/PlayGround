-- @entity: LoginAttemptLogsEntity
CREATE TABLE [dbo].[LoginAttemptLogs]
(
    [AttemptId] BIGINT IDENTITY(1,1) NOT NULL,
    [Email] VARCHAR(255) NOT NULL,
    [IpAddress] VARCHAR(45) NOT NULL,
    [UserAgent] VARCHAR(500) NULL,

    [IsSuccess] BIT NOT NULL,
    [FailureReason] VARCHAR(100) NULL, -- 'InvalidPassword', 'AccountLocked', 'AccountNotFound'

    [AttemptedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_LoginAttemptLogs] PRIMARY KEY ([AttemptId])
);
