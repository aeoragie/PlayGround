-- @entity: UserFamiliesEntity
CREATE TABLE [dbo].[UserFamilies]
(
    [ParentUid] UNIQUEIDENTIFIER NOT NULL,
    [PlayerUid] UNIQUEIDENTIFIER NOT NULL,
    [RelationshipType] VARCHAR(20) NULL, -- 'Father', 'Mother' 등
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_UserFamilies] PRIMARY KEY ([ParentUid], [PlayerUid]),
    CONSTRAINT [FK_UserFamilies_Parent] FOREIGN KEY ([ParentUid]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [FK_UserFamilies_Child] FOREIGN KEY ([PlayerUid]) REFERENCES [dbo].[Users]([UserId])
);