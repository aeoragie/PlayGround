-- @entity: PlayerEvaluationsEntity
-- 선수 본인 자기분석 + 지도자 공식 평가 (ScoutingMemos와 별개)
CREATE TABLE [dbo].[PlayerEvaluations]
(
    [EvaluationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,

    [EvaluatorId] UNIQUEIDENTIFIER NULL,     -- NULL = 본인 평가, NOT NULL = 지도자 평가
    [EvaluatorRole] VARCHAR(20) NULL,        -- 'Self','Coach','Agent'
    [Season] VARCHAR(20) NULL,

    [Strengths] NVARCHAR(1000) NULL,         -- 강점
    [Weaknesses] NVARCHAR(1000) NULL,        -- 보완할 점
    [TargetPosition] VARCHAR(5) NULL,        -- 목표 포지션
    [OverallComment] NVARCHAR(2000) NULL,    -- 종합 평가

    -- 선수에게 공개 여부 (지도자 평가 시)
    [IsVisibleToPlayer] BIT NOT NULL DEFAULT 1,

    [EvaluatedAt] DATE NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_PlayerEvaluations] PRIMARY KEY ([EvaluationId]),
    CONSTRAINT [FK_PlayerEvaluations_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId])
);
