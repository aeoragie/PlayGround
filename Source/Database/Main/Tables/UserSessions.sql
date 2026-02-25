-- @entity: UserSessionsEntity
CREATE TABLE [dbo].[UserSessions]
(
    [SessionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RefreshTokenId] UNIQUEIDENTIFIER NULL,

    [IpAddress] VARCHAR(45) NULL,
    [UserAgent] VARCHAR(500) NULL,
    [DeviceType] VARCHAR(50) NULL,

    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastActivityAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 NOT NULL,

    CONSTRAINT [PK_UserSessions] PRIMARY KEY ([SessionId]),
    CONSTRAINT [FK_UserSessions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [FK_UserSessions_RefreshToken] FOREIGN KEY ([RefreshTokenId]) REFERENCES [dbo].[UserRefreshTokens]([TokenId])
);
