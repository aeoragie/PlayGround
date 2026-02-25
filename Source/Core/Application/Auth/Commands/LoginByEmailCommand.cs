using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 이메일 로그인 유즈케이스
    /// </summary>
    public class LoginByEmailCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;

        private const int MaxFailedLoginCount = 5;
        private const int RefreshTokenExpirationDays = 7;

        public LoginByEmailCommand(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<LoginResult>> ExecuteAsync(LoginRequest request, string? ipAddress, string? userAgent)
        {
            Debug.Assert(request != null, "LoginRequest cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<LoginResult>.Error(ErrorCode.MissingRequired, "Email is required");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return Result<LoginResult>.Error(ErrorCode.MissingRequired, "Password is required");
            }

            var user = await Repository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Result<LoginResult>.Error(ErrorCode.InvalidCredentials, "Invalid email or password");
            }

            // 계정 잠금 확인
            if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > DateTime.UtcNow)
            {
                return Result<LoginResult>.Error(ErrorCode.AccountLocked, "Account is locked. Please try again later.");
            }

            // 계정 상태 확인
            if (user.UserStatus != "Active")
            {
                return Result<LoginResult>.Error(ErrorCode.AccountDisabled, "Account is not active");
            }

            // 비밀번호 검증
            var passwordHash = TokenService.HashToken(request.Password);
            if (user.PasswordHash != passwordHash)
            {
                return Result<LoginResult>.Error(ErrorCode.InvalidCredentials, "Invalid email or password");
            }

            // JWT Access Token 생성
            var accessToken = TokenService.GenerateAccessToken(
                user.UserId, user.Email, user.FullName, user.UserRole, user.ProfileImageUrl);

            // Refresh Token 생성
            var rawRefreshToken = TokenService.GenerateRefreshToken();
            var refreshTokenHash = TokenService.HashToken(rawRefreshToken);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

            var tokenResult = await Repository.CreateRefreshTokenAsync(
                user.UserId, refreshTokenHash, userAgent, ipAddress, refreshTokenExpiry);

            if (tokenResult == null)
            {
                Debug.Assert(false, "Failed to create refresh token");
                return Result<LoginResult>.Error(ErrorCode.DatabaseError, "Failed to create session");
            }

            // 로그인 기록 업데이트
            await Repository.UpdateLastLoginAsync(
                user.UserId, ipAddress, userAgent, null, tokenResult.TokenId, refreshTokenExpiry);

            return Result<LoginResult>.Success(new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = rawRefreshToken,
                ExpiresIn = 900,
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                UserRole = user.UserRole,
                ProfileImageUrl = user.ProfileImageUrl
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }
}
