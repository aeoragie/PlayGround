-- @entity: PlayerPrivacySettingsEntity
-- 선수 프로필 항목별 공개/비공개 설정 (보호자 권한)
CREATE TABLE [dbo].[PlayerPrivacySettings]
(
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    -- 각 항목의 공개 수준: 'Public','TeamOnly','Private','RequestOnly'
    [ProfileVisibility] VARCHAR(20) NOT NULL DEFAULT 'Public',
    [ContractVisibility] VARCHAR(20) NOT NULL DEFAULT 'Private',
    [StatisticsVisibility] VARCHAR(20) NOT NULL DEFAULT 'Public',
    [PhysicalVisibility] VARCHAR(20) NOT NULL DEFAULT 'TeamOnly',
    [SkillRatingVisibility] VARCHAR(20) NOT NULL DEFAULT 'TeamOnly',
    [SelfAnalysisVisibility] VARCHAR(20) NOT NULL DEFAULT 'Private',
    [PortfolioVisibility] VARCHAR(20) NOT NULL DEFAULT 'Public',
    [GoalsVisibility] VARCHAR(20) NOT NULL DEFAULT 'Private',
    [CareerPathVisibility] VARCHAR(20) NOT NULL DEFAULT 'Private',
    [TrainingLogVisibility] VARCHAR(20) NOT NULL DEFAULT 'TeamOnly',
    [ScheduleVisibility] VARCHAR(20) NOT NULL DEFAULT 'TeamOnly',

    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PlayerPrivacySettings] PRIMARY KEY ([PlayerId]),
    CONSTRAINT [FK_PlayerPrivacySettings_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId])
);
