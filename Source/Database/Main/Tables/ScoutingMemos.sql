-- @entity: ScoutingMemosEntity
-- 코치/에이전트 전용 비공개 메모 (선수 본인 열람 불가)
CREATE TABLE [dbo].[ScoutingMemos]
(
    [ScoutingMemoId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlayerId] UNIQUEIDENTIFIER NOT NULL,
    [AuthorId] UNIQUEIDENTIFIER NOT NULL,

    [AuthorRole] VARCHAR(20) NOT NULL,       -- 'Coach','Agent'
    [Content] NVARCHAR(2000) NOT NULL,
    [Tags] NVARCHAR(500) NULL,               -- '기술우수,멘탈강점,성장가능성'

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_ScoutingMemos] PRIMARY KEY ([ScoutingMemoId]),
    CONSTRAINT [FK_ScoutingMemos_Players] FOREIGN KEY ([PlayerId]) REFERENCES [dbo].[Players]([PlayerId]),
    CONSTRAINT [FK_ScoutingMemos_Authors] FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Users]([UserId])
);
