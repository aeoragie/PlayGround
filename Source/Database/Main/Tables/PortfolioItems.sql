-- @entity: PortfolioItemsEntity
CREATE TABLE [dbo].[PortfolioItems]
(
    [PortfolioItemId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [ItemType] VARCHAR(20) NOT NULL,         -- 'Video','Article','Link'
    [Title] NVARCHAR(200) NOT NULL,
    [Url] VARCHAR(2048) NOT NULL,
    [ThumbnailUrl] VARCHAR(2048) NULL,
    [Description] NVARCHAR(500) NULL,
    [Tags] NVARCHAR(500) NULL,               -- 쉼표 구분: '골,프리킥,드리블'
    [IsFeatured] BIT NOT NULL DEFAULT 0,     -- 대표 하이라이트

    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PortfolioItems] PRIMARY KEY ([PortfolioItemId]),
    CONSTRAINT [FK_PortfolioItems_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId])
);
