-- =============================================================
-- 계정/인증 관련 인덱스
-- =============================================================

-- Users: 상태별 조회 (Soft Delete 제외)
CREATE INDEX [IX_Users_Status]
ON [dbo].[Users] ([UserStatus])
WHERE [DeletedAt] IS NULL;
GO

-- Users: 역할별 조회
CREATE INDEX [IX_Users_Role]
ON [dbo].[Users] ([UserRole])
WHERE [DeletedAt] IS NULL;
GO

-- UserSessions: 활성 세션 조회
CREATE INDEX [IX_UserSessions_User_Active]
ON [dbo].[UserSessions] ([UserId], [IsActive])
WHERE [IsActive] = 1;
GO

-- UserRefreshTokens: 유효 토큰 조회
CREATE INDEX [IX_UserRefreshTokens_User_Valid]
ON [dbo].[UserRefreshTokens] ([UserId])
WHERE [IsRevoked] = 0;
GO

-- UserRefreshTokens: 토큰 값으로 조회
CREATE INDEX [IX_UserRefreshTokens_Token]
ON [dbo].[UserRefreshTokens] ([Token]);
GO

-- EmailVerifications: 이메일+용도별 조회
CREATE INDEX [IX_EmailVerifications_Email_Purpose]
ON [dbo].[EmailVerifications] ([Email], [Purpose])
WHERE [IsVerified] = 0;
GO

-- PasswordResetTokens: 사용자별 미사용 토큰 조회
CREATE INDEX [IX_PasswordResetTokens_User_Unused]
ON [dbo].[PasswordResetTokens] ([UserId])
WHERE [UsedAt] IS NULL;
GO

-- LoginAttemptLogs: IP별 최근 시도 조회 (Rate Limiting)
CREATE INDEX [IX_LoginAttemptLogs_Ip_Time]
ON [dbo].[LoginAttemptLogs] ([IpAddress], [AttemptedAt]);
GO

-- LoginAttemptLogs: 이메일별 최근 시도 조회
CREATE INDEX [IX_LoginAttemptLogs_Email_Time]
ON [dbo].[LoginAttemptLogs] ([Email], [AttemptedAt]);
GO

-- AuditLogs: 사용자별 조회
CREATE INDEX [IX_AuditLogs_User]
ON [dbo].[AuditLogs] ([UserId], [PerformedAt]);
GO

-- AuditLogs: 액션별 조회
CREATE INDEX [IX_AuditLogs_Action]
ON [dbo].[AuditLogs] ([Action], [PerformedAt]);
GO

-- UserSocialAccounts: 사용자별 연결된 소셜 계정 조회
CREATE INDEX [IX_UserSocialAccounts_User]
ON [dbo].[UserSocialAccounts] ([UserId]);
GO

-- UserFamilies: 부모로 자녀 조회
CREATE INDEX [IX_UserFamilies_Parent]
ON [dbo].[UserFamilies] ([ParentUid]);
GO

-- UserFamilies: 자녀로 부모 조회
CREATE INDEX [IX_UserFamilies_Player]
ON [dbo].[UserFamilies] ([PlayerUid]);
GO
