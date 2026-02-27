# PlayGround 프로젝트

축구 선수 스카우팅 및 매칭 플랫폼

## 프로젝트 구조

```
Source/
├── Core/                              (재사용 가능한 범용 레이어 - PlayGround 비종속)
│   ├── Shared/                        범용 .NET 유틸리티 (도메인 무관, 의존성 없음) → Core.Shared.csproj
│   │   ├── Http/                      Envelope<T>, PagedData<T>
│   │   ├── Result/                    Result<T> 모나드 패턴
│   │   │   └── Codes/                 범용 DetailCode, ErrorCode, SuccessCode 등
│   │   ├── Extensions/               문자열, Enum, 변환 확장 메서드
│   │   └── Validation/               범용 검증 유틸리티
│   │
│   └── Infrastructure/                외부 라이브러리 래핑 + DB 기반 라이브러리 (Shared 참조) → Core.Infrastructure.csproj
│       ├── Database/
│       │   └── Base/                  CommandBase, ProcedureBase, QueryBase, RepositoryBase, ResultBase
│       ├── Store/                     Redis 래핑
│       ├── Actor/                     Akka 래핑
│       ├── Email/                     이메일 서비스
│       └── Logging/                   NLog 설정/래핑
│
├── PlayGround/                        (PlayGround 프로젝트 전용 레이어)
│   ├── PlayGround.Domain/             비즈니스 규칙, 엔티티, 도메인 Enum (Core.Shared 참조) → PlayGround.Domain.csproj
│   │   ├── Enums/                     도메인 Enum (PlayerCategory, MatchStatus 등)
│   │   │   └── Soccer/               축구 전용 Enum (SoccerPosition, PreferredFoot 등)
│   │   ├── Codes/                     도메인 특화 ResultCode (SportsErrorCode 등)
│   │   └── SubDomains/                서브도메인별 엔티티, 값 객체
│   │
│   ├── PlayGround.Application/        유즈케이스, 서비스 오케스트레이션 (Domain, Core.Shared 참조) → PlayGround.Application.csproj
│   │   ├── {기능}/Commands/           상태 변경 유즈케이스 (생성, 수정, 삭제)
│   │   ├── {기능}/Queries/            조회 유즈케이스
│   │   ├── Interfaces/                인프라 포트 (IEmailService 등)
│   │   ├── Mappers/                   Entity ↔ DTO 변환
│   │   └── Validators/                입력값 검증
│   │
│   ├── PlayGround.Persistence/        DB 전용 (Domain, Application, Core.Shared, Core.Infrastructure 참조) → PlayGround.Persistence.csproj
│   │   ├── Database/
│   │   │   └── Generated/            Generator.Database 생성 코드 출력 위치
│   │   │       └── {DB명}/
│   │   │           ├── Entities/      테이블 엔티티
│   │   │           ├── Procedures/    프로시저
│   │   │           └── Queries/       쿼리
│   │   ├── Data/                      EF Core DbContext, Migrations
│   │   └── Repositories/              리포지토리 구현체
│   │
│   ├── PlayGround.Server/             ASP.NET Core API Server (모든 레이어 참조) → PlayGround.Server.csproj
│   └── PlayGround.Client/             Blazor WebAssembly 프론트엔드 (Core.Shared 참조) → PlayGround.Client.csproj
│       ├── Layout/                    MainLayout, DashboardLayout
│       ├── Pages/                     Auth, Team, Player, Agent
│       ├── Components/                KpiCard, StatusBadge 등 재사용 컴포넌트
│       ├── Services/                  API 통신 서비스
│       ├── Auth/                      인증 상태 관리
│       └── Styles/                    Tailwind CSS 소스
│
├── Database/
│   └── Main/                          SQL 테이블/프로시저 정의
│       ├── Schema/                    스키마 정의
│       ├── Tables/                    테이블 DDL
│       ├── Procedures/                저장 프로시저
│       ├── Queries/                   쿼리 정의
│       └── Indexes/                   인덱스 정의
│
├── AppHost/                           .NET Aspire 호스트 (오케스트레이션) → PlayGround.AppHost.csproj
├── ServiceDefaults/                   서비스 기본 설정 (OpenTelemetry, HealthCheck) → PlayGround.ServiceDefaults.csproj
│
└── Tools/
    └── Generator.Database/            데이터베이스 코드 생성기 → Persistence 출력

Tests/
├── Tests.Unit/                        단위 테스트 (Domain, Application, Core.Shared)
├── Tests.Integration/                 통합 테스트 (API 엔드포인트)
└── Tests.Infrastructure/              인프라 테스트 (DB, 외부 서비스)
```

## 기술 스택

- **.NET 9.0**
- **Blazor WebAssembly** (SPA 프론트엔드)
- **Tailwind CSS** (유틸리티 기반 스타일링)
- **ASP.NET Core Web API** (REST API 서버)
- **Entity Framework Core 9.x** (CRUD, 마이그레이션)
- **Dapper** (복잡한 SP 호출, 고성능 조회)
- **.NET Aspire** (오케스트레이션, ServiceDiscovery, OpenTelemetry)
- **SignalR** (실시간 통신)
- **ASP.NET Core Identity + JWT** (인증/인가)
- **SQL Server + Redis** (데이터 저장 + 캐시)
- **NLog** (로깅)
- **xUnit, Moq, FluentAssertions** (테스트)
- **Chart.js** (JS Interop 차트)

## 아키텍처

Clean Architecture 기반, Core(재사용)와 PlayGround(프로젝트 전용)로 분리:

**Core 레이어** (PlayGround 비종속, 다른 프로젝트에서도 재사용 가능):
- **Core.Shared**: 범용 .NET 유틸리티 라이브러리 (의존성 없음, 도메인 무관)
  - 어떤 프로젝트에서든 재사용 가능한 코드: Result<T>, Envelope<T>, Extensions
  - 도메인 특화 코드 포함 금지
- **Core.Infrastructure**: 외부 라이브러리 래핑 + DB 기반 라이브러리 (Core.Shared만 참조)
  - DB 기반 클래스: RepositoryBase, CommandBase, ProcedureBase, QueryBase 등
  - Redis, Akka 등 외부 라이브러리를 프레임워크에 맞게 래핑/확장
  - Domain/Application 참조 금지 (PlayGround 비종속)

**PlayGround 레이어** (프로젝트 전용):
- **PlayGround.Domain**: 비즈니스 규칙, 엔티티, 도메인 Enum, 도메인 ResultCode (Core.Shared 참조)
  - 외부 라이브러리 의존 금지, 순수 비즈니스 로직만 포함
  - 도메인 Enum, SportsErrorCode 등 도메인 특화 코드 위치
- **PlayGround.Application**: 유즈케이스 오케스트레이션 (Domain, Core.Shared 참조)
  - API 하나가 하는 일 = 유즈케이스 하나 (Command/Query 패턴)
  - 인프라 인터페이스 정의 (포트), 구현은 Persistence에서
- **PlayGround.Persistence**: DB 전용 (Application, Domain, Core.Shared, Core.Infrastructure 참조)
  - Generated 코드, EF Core DbContext, 리포지토리 구현체
- **PlayGround.Server**: ASP.NET Core API (모든 레이어 참조)
- **PlayGround.Client**: Blazor WebAssembly 프론트엔드 (Core.Shared 참조)

### 의존성 그래프

```
Core.Shared (의존성 없음)
  ↑
Core.Infrastructure (Core.Shared 참조)
  ↑
PlayGround.Domain (Core.Shared 참조)
  ↑
PlayGround.Application (PlayGround.Domain, Core.Shared 참조)
  ↑
PlayGround.Persistence (PlayGround.Application, PlayGround.Domain, Core.Shared, Core.Infrastructure 참조)
  ↑
PlayGround.Server (모든 레이어 참조)
```

### 데이터 흐름 패턴
- **내부 로직**: `Result<T>` 모나드 패턴으로 함수형 에러 처리
- **API 응답**: `Envelope<T>` + `PagedData<T>` 래퍼

### 빌드 구성
- `Directory.Build.props`: 빌드 출력 경로 중앙 관리 (Binary/, Intermediate/)
- `Directory.Packages.props`: NuGet 패키지 버전 중앙 관리
- `.editorconfig`: 코딩 스타일 규칙

---

# 코딩 컨벤션

## C# 코딩 스타일

### 네이밍 규칙

- **클래스, 메서드, 속성**: PascalCase (예: `CustomerService`, `GetCustomerById`)
- **변수, 필드**: camelCase (예: `customerId`, `orderItems`)
- **상수**: PascalCase (예: `DefaultPageSize`, `CacheKeyPrefix`)
- **private 멤버 변수**: `m` 접두사 + PascalCase (예: `mConnectionString`, `mIsInitialized`)
- **readonly 필드**: PascalCase (예: `Repository`, `Logger`)
- **static 멤버 변수**: PascalCase (예: `Repository`, `Logger`)

### 포매팅 규칙

- **들여쓰기**: 4개 공백
- **중괄호**: Allman 스타일 (여는 중괄호를 새 줄에)
- **using 문**: 파일 상단에 배치
- **네임스페이스**: block_scoped (`namespace Foo { }`)
- **LINQ 체이닝**: 메서드 체이닝 시 각 메서드는 새 줄에 작성, 들여쓰기는 첫 번째 메서드와 동일한 레벨 유지

### using 지시문 순서 규칙

그룹 순서: **System → Microsoft → 3rd Party → Core → PlayGround**
- 그룹 간 빈 줄 없음
- 같은 그룹 내: 알파벳순 정렬
- Core/PlayGround 그룹 내: 의존성 낮은 순 (같은 뎁스는 알파벳순)

```csharp
// 1. System (알파벳순)
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

// 2. Microsoft (알파벳순)
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// 3. 3rd Party (알파벳순)
using Dapper;
using NLog;

// 4. Core (의존성 낮은 순, 같은 뎁스는 알파벳순)
//    Core.Shared → Core.Infrastructure 순
using PlayGround.Shared.Http;
using PlayGround.Shared.Result;
using PlayGround.Infrastructure.Database;

// 5. PlayGround (의존성 낮은 순, 같은 뎁스는 알파벳순)
//    Domain → Application → Persistence → Server 순
using PlayGround.Application.Interfaces;
using PlayGround.Domain.Enums.Soccer;
using PlayGround.Server.Services;
```

### 예외 처리 스타일

```csharp
// 올바른 스타일 - Allman 스타일
try
{
    // 코드 실행
}
catch (Exception ex)
{
    // 예외 처리
}
```

### Debug.Assert 사용 규칙

- **조건 검증**: 모든 중요한 조건에서 Debug.Assert() 사용
- **예외 상황**: 예상하지 못한 상황에서 Debug.Assert(false, "설명") 사용
- **null 체크**: 중요한 객체의 null 체크 후 Assert 추가

```csharp
// 조건 검증 예시
public void ProcessData(string data)
{
    Debug.Assert(!string.IsNullOrEmpty(data), "Data cannot be null or empty");

    if (string.IsNullOrEmpty(data))
    {
        throw new ArgumentNullException(nameof(data));
    }

    // 처리 로직...
}

// 예외 상황 예시
PropertyInfo? prop = type.GetProperty(name);
if (prop == null)
{
    Debug.Assert(false, $"Property '{name}' not found in type '{type.Name}'");
    return null;
}
```

### 코드 예시

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PlayGround.Application.Services
{
    public class PlayerService
    {
        private readonly IPlayerRepository Repository;
        private readonly ILogger<PlayerService> Logger;
        private readonly int MaxRetryCount;

        private const int DefaultPageSize = 20;
        private const string CacheKeyPrefix = "PLAYER_";

        private string mConnectionString;
        private bool mIsInitialized;

        public PlayerService(
            IPlayerRepository repository,
            ILogger<PlayerService> logger,
            IConfiguration configuration)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            MaxRetryCount = configuration.GetValue<int>("MaxRetryCount");
            mConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<Result<PlayerDto>> GetPlayerByIdAsync(int playerId)
        {
            if (playerId <= 0)
            {
                throw new ArgumentException("Player ID must be greater than zero", nameof(playerId));
            }

            try
            {
                var player = await Repository.GetByIdAsync(playerId);
                if (player == null)
                {
                    Debug.Assert(false, $"Player not found: {playerId}");
                    return Result<PlayerDto>.Fail(ErrorCode.NotFound);
                }

                Debug.Assert(player.Id == playerId, "Retrieved player ID does not match requested ID");
                return Result<PlayerDto>.Success(new PlayerDto { Id = player.Id, Name = player.Name });
            }
            catch (DataAccessException ex)
            {
                Logger.LogError(ex, "Failed to retrieve player with ID: {PlayerId}", playerId);
                return Result<PlayerDto>.Fail(ErrorCode.DatabaseError);
            }
        }
    }
}
```

### LINQ 체이닝 스타일

```csharp
// 올바른 스타일 - 각 메서드를 새 줄에, 동일한 들여쓰기 레벨
public IReadOnlyList<PlayerDto> GetPlayersByPosition(PlayerPosition position)
{
    return Players.Where(p => p.Position == position && p.IsActive)
        .OrderByDescending(p => p.Rating)
        .ThenByDescending(p => p.MatchCount)
        .ToList()
        .AsReadOnly();
}

// 잘못된 스타일 - 계속 들여쓰기가 증가
public IReadOnlyList<PlayerDto> GetActivePlayers(int count = 10)
{
    return Players.Where(p => p.IsActive)
                  .OrderByDescending(p => p.Rating)  // X - 잘못된 들여쓰기
                  .Take(count)
                  .ToList()
                  .AsReadOnly();
}
```

## Blazor 컴포넌트

- **파일명**: PascalCase로 명명 (예: `KpiCard.razor`, `StatusBadge.razor`)
- **컴포넌트 구조**: 마크업, 코드블록 순서로 배치
- **매개변수**: `[Parameter]` 특성과 함께 public 속성으로 정의
- **이벤트 콜백**: `EventCallback` 또는 `EventCallback<T>` 사용
- **CSS**: Tailwind CSS 유틸리티 클래스 사용

### Tailwind CSS 디자인 토큰

| 토큰 | 클래스 | 값 | 용도 |
|------|--------|------|------|
| Primary | `bg-primary` | `#FF6B35` | CTA, 활성 상태, 강조 |
| Secondary | `bg-secondary` | `#2EC4B6` | 보조 버튼, 성공 |
| Background | `bg-bg` | `#F8F6F3` | 페이지 배경 |
| Surface | `bg-surface` | `#FFFFFF` | 카드, 패널 배경 |
| Border | `border-border` | `#E8E3DD` | 카드 테두리, 구분선 |
| Text Primary | `text-text-primary` | `#1A1D21` | 제목, 본문 |
| Text Light | `text-text-light` | `#A0A7AE` | 보조 텍스트 |
| Card Radius | `rounded-card` | `14px` | 카드, 패널 |
| Button Radius | `rounded-btn` | `8px` | 버튼, 입력 필드 |

### Tailwind 컴포넌트 클래스

```css
.btn-primary   → bg-primary text-white font-bold text-xs px-4 py-2 rounded-btn
.btn-ghost     → bg-transparent text-text-secondary border border-border
.panel         → bg-surface rounded-card border border-border p-5
.kpi-card      → bg-surface rounded-xl border border-border px-5 py-4
```

## 데이터베이스

- **테이블명**: PascalCase 복수형 (예: `Players`, `Teams`)
- **컬럼명**: PascalCase 단수형 (예: `PlayerId`, `TeamName`)
- **프로시저**: `Usp` 접두사 사용 (User Stored Procedure)
- **ORM 전략** (PlayGround.Persistence):
  - EF Core: CRUD 작업, 마이그레이션
  - Dapper: 복잡한 SP 호출, 성능 중요 조회

## 빌드 & 테스트

- **빌드**: `dotnet build PlayGround.sln`
- **실행 (Aspire)**: `dotnet run --project Source/AppHost`
- **실행 (서버만)**: `dotnet run --project Source/PlayGround/PlayGround.Server`
- **테스트**: `dotnet test`
- **Tailwind 빌드**: `cd Source/PlayGround/PlayGround.Client && npx tailwindcss -i ./Styles/app.tailwind.css -o ./wwwroot/css/app.css`

## 중요 규칙

- **민감정보 로깅 금지**: 패스워드, API 키 등 민감한 정보 로그 기록 금지
- **보안 모범사례 준수**: SQL Injection, XSS 방지 등
- **기존 코드 패턴 따르기**: 프로젝트 내 일관성 유지
- **null 체크 필수**: ArgumentNullException 및 Debug.Assert 사용
- **예외 처리 철저히**: try-catch 블록과 Debug.Assert 조합 사용
- **Debug.Assert 필수**: 조건 검증, 예외 상황, 데이터 무결성 검증에 사용

## 코딩 품질 규칙

- **모든 public 메서드**: 매개변수 유효성 검증 + Debug.Assert
- **모든 예외 케이스**: Debug.Assert(false, "상황 설명") 추가
- **모든 중요한 조건**: Debug.Assert로 조건 확인
- **try-catch 스타일**: catch는 반드시 새 줄에 작성 (Allman 스타일)
- **리플렉션 사용 시**: 각 단계마다 null 체크 + Debug.Assert

## 로깅 & 주석

- **주석**: 한글로 작성, 간결하게. 함수/클래스/변수명으로 알 수 있으면 주석 불필요
- **로그 메시지**: 영어로 작성 (`Logger.LogError(ex, "Failed to retrieve player")`)
- **예외 메시지**: 영어로 작성

## Claude 작업 규칙

- **빌드 및 에러 수정**: 코드 작업 후 빌드 및 에러 수정은 사용자가 직접 수행 (Claude가 하지 않음)
- **최소 기능 구현**: 요청한 기능만 정확히 구현, 복잡한 추가 코드 생성 금지
- **추가 기능 제안**: 필요시 추가 기능은 코드 작성 없이 제안만 함
