-- @entity: UserSocialAccountsEntity
CREATE TABLE [dbo].[UserSocialAccounts]
(
    [SocialAccountId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Provider] VARCHAR(50) NOT NULL, -- 'Google', 'Kakao', 'Apple'
    [ProviderUserId] VARCHAR(255) NOT NULL, -- 소셜 업체 제공 고유 ID
    [EncryptedAccessToken] VARBINARY(MAX) NULL, -- 보안을 위해 암호화 저장 권장
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_UserSocialAccounts] PRIMARY KEY ([SocialAccountId]),
    CONSTRAINT [FK_UserSocialAccounts_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [UQ_UserSocialAccounts_Provider_Identifier] UNIQUE ([Provider], [ProviderUserId])
);