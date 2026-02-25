-- @entity: EmailVerificationsEntity
CREATE TABLE [dbo].[EmailVerifications]
(
    [VerificationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Email] VARCHAR(255) NOT NULL,
    [VerificationCode] VARCHAR(10) NOT NULL,
    [Purpose] VARCHAR(20) NOT NULL, -- 'SignUp', 'PasswordReset', 'EmailChange'

    -- Rate Limiting
    [RequestIp] VARCHAR(45) NULL,
    [AttemptCount] INT NOT NULL DEFAULT 0,

    -- 상태
    [IsVerified] BIT NOT NULL DEFAULT 0,
    [VerifiedAt] DATETIME2 NULL,

    [ExpiresAt] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT [PK_EmailVerifications] PRIMARY KEY ([VerificationId])
);
