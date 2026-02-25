-- @entity: AuditLogsEntity
CREATE TABLE [dbo].[AuditLogs]
(
    [AuditLogId] BIGINT IDENTITY(1,1) NOT NULL,
    [UserId] UNIQUEIDENTIFIER NULL, -- 비로그인 상태 액션일 수 있음
    [Action] VARCHAR(100) NOT NULL, -- 'LOGIN', 'UPDATE_PROFILE'
    [TableName] VARCHAR(100) NULL,
    [EntityId] VARCHAR(255) NULL,
    [OldValues] VARCHAR(MAX) NULL,
    [NewValues] VARCHAR(MAX) NULL,
    [PerformedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([AuditLogId])
);