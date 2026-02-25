-- @entity: PasswordResetTokensEntity
CREATE TABLE [dbo].[PasswordResetTokens]
(
    [TokenId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [TokenHash] VARCHAR(255) NOT NULL, -- 토큰은 해시로 저장

    [RequestIp] VARCHAR(45) NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [UsedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([TokenId]),
    CONSTRAINT [FK_PasswordResetTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);
