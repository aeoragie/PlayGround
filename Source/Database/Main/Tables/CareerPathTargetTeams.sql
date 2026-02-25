-- @entity: CareerPathTargetTeamsEntity
-- 관심 팀 위시리스트
CREATE TABLE [dbo].[CareerPathTargetTeams]
(
    [CareerPathTargetTeamId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CareerPathId] UNIQUEIDENTIFIER NOT NULL,
    [TeamId] UNIQUEIDENTIFIER NULL,

    [TeamName] NVARCHAR(100) NOT NULL,       -- 외부 팀일 수 있으므로 이름 직접 저장
    [LeagueName] NVARCHAR(100) NULL,
    [Priority] INT NOT NULL DEFAULT 0,       -- 관심 순위
    [Notes] NVARCHAR(300) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_CareerPathTargetTeams] PRIMARY KEY ([CareerPathTargetTeamId]),
    CONSTRAINT [FK_CareerPathTargetTeams_CareerPaths] FOREIGN KEY ([CareerPathId]) REFERENCES [dbo].[CareerPaths]([CareerPathId]),
    CONSTRAINT [FK_CareerPathTargetTeams_Teams] FOREIGN KEY ([TeamId]) REFERENCES [dbo].[Teams]([TeamId])
);
