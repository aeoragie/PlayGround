-- @param: UserId UNIQUEIDENTIFIER NULL
-- @param: Action VARCHAR(100)
-- @param: TableName VARCHAR(100) NULL
-- @param: EntityId VARCHAR(255) NULL
-- @param: OldValues VARCHAR(MAX) NULL
-- @param: NewValues VARCHAR(MAX) NULL
INSERT INTO [dbo].[AuditLogs]
(
    [UserId],
    [Action],
    [TableName],
    [EntityId],
    [OldValues],
    [NewValues],
    [PerformedAt]
)
VALUES
(
    @UserId,
    @Action,
    @TableName,
    @EntityId,
    @OldValues,
    @NewValues,
    GETUTCDATE()
);
