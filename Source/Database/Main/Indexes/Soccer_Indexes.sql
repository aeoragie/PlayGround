-- =============================================================
-- dbo 테이블 ExternalId 검색 (크롤링 임포트 시 중복 체크)
-- =============================================================

CREATE UNIQUE INDEX [IX_Competitions_ExternalId]
ON [dbo].[Competitions]([ExternalId])
WHERE [ExternalId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Matches_ExternalId]
ON [dbo].[Matches]([ExternalId])
WHERE [ExternalId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Teams_ExternalId]
ON [dbo].[Teams]([ExternalId])
WHERE [ExternalId] IS NOT NULL;
GO

-- =============================================================
-- dbo 테이블 SportType 필터
-- =============================================================

CREATE INDEX [IX_Competitions_SportType]
ON [dbo].[Competitions]([SportType]);
GO

CREATE INDEX [IX_Matches_SportType]
ON [dbo].[Matches]([SportType]);
GO

CREATE INDEX [IX_Teams_SportType]
ON [dbo].[Teams]([SportType]);
GO

-- =============================================================
-- dbo.Matches 대회별 경기 목록 (날짜순)
-- =============================================================

CREATE INDEX [IX_Matches_CompetitionId_MatchDate]
ON [dbo].[Matches]([CompetitionId], [MatchDate]);
GO

-- =============================================================
-- Soccer 스키마 테이블 조회 최적화
-- =============================================================

CREATE INDEX [IX_Soccer_MatchEvents_MatchId]
ON [Soccer].[MatchEvents]([MatchId]);
GO

CREATE INDEX [IX_Soccer_MatchLineups_MatchId]
ON [Soccer].[MatchLineups]([MatchId]);
GO

CREATE INDEX [IX_Soccer_CompetitionTeams_CompetitionId]
ON [Soccer].[CompetitionTeams]([CompetitionId]);
GO

CREATE INDEX [IX_Soccer_Standings_CompetitionId]
ON [Soccer].[Standings]([CompetitionId]);
GO
