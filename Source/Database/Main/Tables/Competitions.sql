-- @entity: CompetitionsEntity
CREATE TABLE [dbo].[Competitions]
(
    [CompetitionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CompetitionName] NVARCHAR(200) NOT NULL,
    [CompetitionType] VARCHAR(20) NOT NULL,  -- 'League', 'Cup', 'Tournament', 'Friendly'
    [SportType] VARCHAR(20) NOT NULL DEFAULT 'Soccer',  -- 'Soccer', 'Basketball', 'Baseball'
    [Season] VARCHAR(20) NOT NULL,           -- '2025-2026'
    [Format] VARCHAR(20) NULL,               -- '리그', '대회' (크롤링 StyleNm)
    [AgeGroup] VARCHAR(20) NULL,             -- 'U12', 'U15', 'U17', 'U20', 'Senior'
    [Region] NVARCHAR(100) NULL,
    [Organizer] NVARCHAR(200) NULL,
    [StartDate] DATE NULL,
    [EndDate] DATE NULL,
    [ExternalId] VARCHAR(64) NULL,           -- 외부 시스템 ID (KFA hex ID 등)

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Competitions] PRIMARY KEY ([CompetitionId])
);
