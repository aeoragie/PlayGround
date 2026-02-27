using System.Diagnostics;
using PlayGround.Application.Interfaces;
using PlayGround.Shared.Result;

namespace PlayGround.Application.Auth.Commands
{
    /// <summary>
    /// 이메일 회원가입 유즈케이스
    /// </summary>
    public class RegisterByEmailCommand
    {
        private readonly IAuthRepository Repository;
        private readonly IJwtTokenService TokenService;

        public RegisterByEmailCommand(IAuthRepository repository, IJwtTokenService tokenService)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<RegisterResult>> ExecuteAsync(RegisterRequest request)
        {
            Debug.Assert(request != null, "RegisterRequest cannot be null");

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return Result<RegisterResult>.Error(ErrorCode.MissingRequired, "Email is required");
            }

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            {
                return Result<RegisterResult>.Error(ErrorCode.InvalidInput, "Password must be at least 8 characters");
            }

            if (string.IsNullOrWhiteSpace(request.FullName))
            {
                return Result<RegisterResult>.Error(ErrorCode.MissingRequired, "FullName is required");
            }

            // 이메일 중복 확인
            var existingUser = await Repository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return Result<RegisterResult>.Error(ErrorCode.AlreadyExists, "Email already registered");
            }

            // 비밀번호 해싱
            var passwordHash = TokenService.HashToken(request.Password);

            var user = await Repository.CreateUserByEmailAsync(
                request.Email,
                passwordHash,
                request.FullName,
                request.Role ?? "Player");

            if (user == null)
            {
                Debug.Assert(false, "Failed to create user");
                return Result<RegisterResult>.Error(ErrorCode.DatabaseError, "Failed to create user");
            }

            return Result<RegisterResult>.Success(new RegisterResult
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                UserRole = user.UserRole
            });
        }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class RegisterResult
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}
