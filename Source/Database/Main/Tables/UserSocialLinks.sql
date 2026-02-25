-- @entity: UserSocialLinksEntity
CREATE TABLE [dbo].[UserSocialLinks]
(
    [SocialLinkId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [PlatformName] VARCHAR(50) NOT NULL, -- 'Instagram', 'YouTube', 'TikTok', 'Blog' 등
    [Url] VARCHAR(2048) NOT NULL,        -- SNS 주소
    [IsPublic] BIT NOT NULL DEFAULT 1,   -- 프로필 노출 여부
    [DisplayOrder] INT NOT NULL DEFAULT 0, -- 정렬 순서
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_UserSocialLinks] PRIMARY KEY ([SocialLinkId]),
    CONSTRAINT [FK_UserSocialLinks_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);
GO
-- 인덱스: 한 유저가 같은 플랫폼 링크를 중복 등록하는 것을 방지
CREATE UNIQUE INDEX [UQ_UserSocialLinks_User_Platform] 
ON [dbo].[UserSocialLinks] ([UserId], [PlatformName]);
