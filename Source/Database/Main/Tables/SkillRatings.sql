-- @entity: SkillRatingsEntity
CREATE TABLE [dbo].[SkillRatings]
(
    [SkillRatingId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [SkillType] VARCHAR(20) NOT NULL,        -- 'BallControl','Dribbling','ShortPass','LongPass','Shooting' 등
    [Score] INT NOT NULL,                    -- 1~100
    [Season] VARCHAR(20) NULL,               -- '2025-2026'
    [EvaluatorId] UNIQUEIDENTIFIER NULL,     -- 평가자 UserId
    [EvaluatedAt] DATE NOT NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_SkillRatings] PRIMARY KEY ([SkillRatingId]),
    CONSTRAINT [FK_SkillRatings_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [CK_SkillRatings_Score] CHECK ([Score] >= 1 AND [Score] <= 100)
);
