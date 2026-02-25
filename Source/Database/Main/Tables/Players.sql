-- @entity: PlayersEntity
CREATE TABLE [dbo].[Players]
(
    [PlayerId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,

    -- 기본 정보
    [PlayerName] NVARCHAR(100) NOT NULL,
    [BirthDate] DATE NOT NULL,
    [Height] DECIMAL(5,1) NULL,              -- cm
    [Weight] DECIMAL(5,1) NULL,              -- kg
    [Nationality] VARCHAR(2) NULL,           -- ISO 3166-1 alpha-2

    -- 축구 정보
    [Position] VARCHAR(5) NOT NULL,          -- 'GK','CB','LB','RB','CDM','CM','CAM','LW','RW','CF','ST'
    [SecondaryPosition] VARCHAR(5) NULL,
    [PreferredFoot] VARCHAR(5) NOT NULL DEFAULT 'Right', -- 'Right','Left','Both'
    [ShirtNumber] INT NULL,

    -- 소속 정보
    [PlayerCategory] VARCHAR(20) NOT NULL DEFAULT 'Club', -- 'Club','School','Unaffiliated'
    [ContractStatus] VARCHAR(20) NOT NULL DEFAULT 'Free',  -- 'Active','Expired','Free','Trial'
    [CurrentTeamId] UNIQUEIDENTIFIER NULL,

    -- 자기소개
    [Bio] NVARCHAR(500) NULL,
    [PlayStyle] NVARCHAR(500) NULL,
    [ProfileImageUrl] VARCHAR(2048) NULL,

    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [DeletedAt] DATETIME2 NULL,

    CONSTRAINT [PK_Players] PRIMARY KEY ([PlayerId]),
    CONSTRAINT [FK_Players_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]),
    CONSTRAINT [FK_Players_CurrentTeam] FOREIGN KEY ([CurrentTeamId]) REFERENCES [dbo].[Teams]([TeamId]),
    CONSTRAINT [UQ_Players_UserId] UNIQUE ([UserId])
);
