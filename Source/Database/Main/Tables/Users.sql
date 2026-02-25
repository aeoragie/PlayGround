-- @entity: UsersEntity
CREATE TABLE [dbo].[Users]
(
    [UserId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [Email] VARCHAR(255) NOT NULL,
    [EmailConfirmed] BIT NOT NULL DEFAULT 0, -- SMALLINT에서 BIT로 개선
    [PasswordHash] VARCHAR(255) NULL,
    
    -- 소셜 로그인 연동 정보 (선택 사항: SocialAccounts 테이블과 병행 가능)
    [AuthProvider] VARCHAR(20) NOT NULL DEFAULT 'Local', 
    
    [FullName] VARCHAR(100) NOT NULL,
    [NickName] VARCHAR(50) NULL,
    [ProfileImageUrl] VARCHAR(2048) NULL,
    [PhoneNumber] VARCHAR(50) NULL,
    [Birthday] DATE NULL,
    [Gender] TINYINT NULL, -- 0:미정, 1:남, 2:여
    
    [CountryCode] CHAR(2) NULL,
    [LanguageCode] VARCHAR(10) NULL DEFAULT 'ko',
    [TimeZone] VARCHAR(50) NULL DEFAULT 'Korea Standard Time',

    [UserStatus] VARCHAR(20) NOT NULL DEFAULT 'Active',
    [UserRole] VARCHAR(20) NOT NULL DEFAULT 'Player',

    -- 약관 동의
    [AgreedToTermsAt] DATETIME2 NULL,
    [AgreedToMarketingAt] DATETIME2 NULL,
    [AgreedToPrivacyAt] DATETIME2 NULL,

    -- 보안: 로그인 실패 추적 (Brute Force 방지)
    [FailedLoginCount] INT NOT NULL DEFAULT 0,
    [LockoutEndAt] DATETIME2 NULL,

    -- 감사 이력
    [LastLoginAt] DATETIME2 NULL,
    [LastLoginIp] VARCHAR(45) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL, -- Soft Delete 지원

    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
);