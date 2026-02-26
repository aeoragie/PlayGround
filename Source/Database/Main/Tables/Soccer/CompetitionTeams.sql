-- @entity: SoccerCompetitionTeamsEntity
CREATE TABLE [Soccer].[CompetitionTeams]
(
    [CompetitionTeamId]  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CompetitionId]      UNIQUEIDENTIFIER NOT NULL,  -- FK → dbo.Competitions
    [TeamId]             UNIQUEIDENTIFIER NOT NULL,   -- FK → dbo.Teams
    [GroupName]          NVARCHAR(20) NULL,            -- 조별 리그 시 조명

    [CreatedAt]          DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_Soccer_CompetitionTeams] PRIMARY KEY ([CompetitionTeamId]),
    CONSTRAINT [FK_Soccer_CompetitionTeams_Competitions] FOREIGN KEY ([CompetitionId])
        REFERENCES [dbo].[Competitions]([CompetitionId]),
    CONSTRAINT [FK_Soccer_CompetitionTeams_Teams] FOREIGN KEY ([TeamId])
        REFERENCES [dbo].[Teams]([TeamId]),
    CONSTRAINT [UQ_Soccer_CompetitionTeams] UNIQUE ([CompetitionId], [TeamId])
);
