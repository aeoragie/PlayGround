-- @param: UserId UNIQUEIDENTIFIER
-- @param: UserStatus VARCHAR(20)
UPDATE [dbo].[Users]
SET
    [UserStatus] = @UserStatus,
    [UpdatedAt]  = GETUTCDATE()
WHERE
    [UserId]    = @UserId
    AND [DeletedAt] IS NULL;
