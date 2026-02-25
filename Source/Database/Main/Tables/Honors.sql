-- @entity: HonorsEntity
CREATE TABLE [dbo].[Honors]
(
    [HonorId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,
    [CompetitionId] UNIQUEIDENTIFIER NULL,

    [HonorType] VARCHAR(20) NOT NULL,        -- 'Individual','Team'
    [Title] NVARCHAR(200) NOT NULL,          -- 'MVP', '득점왕', '우승'
    [CompetitionName] NVARCHAR(200) NULL,
    [Season] VARCHAR(20) NULL,
    [AwardedAt] DATE NULL,
    [ContributionNote] NVARCHAR(500) NULL,   -- 본인 기여도 설명

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Honors] PRIMARY KEY ([HonorId]),
    CONSTRAINT [FK_Honors_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_Honors_Competitions] FOREIGN KEY ([CompetitionId]) REFERENCES [dbo].[Competitions]([CompetitionId])
);
