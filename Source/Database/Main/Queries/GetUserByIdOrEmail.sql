-- @param: UserId UNIQUEIDENTIFIER NULL
-- @param: Email VARCHAR(255) NULL
SELECT
    [UserId],
    [Email],
    [EmailConfirmed],
    [FullName],
    [NickName],
    [ProfileImageUrl],
    [PhoneNumber],
    [UserStatus],
    [UserRole],
    [LastLoginAt],
    [CreatedAt]
FROM
    [dbo].[Users] WITH (NOLOCK)
WHERE
    [DeletedAt] IS NULL
    AND (
        (@UserId IS NOT NULL AND [UserId] = @UserId)
        OR (@Email IS NOT NULL AND [Email] = @Email)
    );
