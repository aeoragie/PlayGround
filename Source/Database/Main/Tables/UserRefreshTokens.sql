-- @entity: UserRefreshTokensEntity
CREATE TABLE [dbo].[UserRefreshTokens]
(
    [TokenId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Token] VARCHAR(500) NOT NULL,

    -- 보안: 토큰 발급 환경 추적
    [DeviceInfo] VARCHAR(255) NULL,
    [IpAddress] VARCHAR(45) NULL,

    -- 토큰 로테이션 추적
    [ReplacedByTokenId] UNIQUEIDENTIFIER NULL,

    -- 토큰 상태
    [IsRevoked] BIT NOT NULL DEFAULT 0,
    [RevokedAt] DATETIME2 NULL,
    [RevokedReason] VARCHAR(100) NULL,

    [ExpiresAt] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_UserRefreshTokens] PRIMARY KEY ([TokenId]),
    CONSTRAINT [FK_UserRefreshTokens_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [FK_UserRefreshTokens_ReplacedBy] FOREIGN KEY ([ReplacedByTokenId]) REFERENCES [dbo].[UserRefreshTokens]([TokenId])
);
