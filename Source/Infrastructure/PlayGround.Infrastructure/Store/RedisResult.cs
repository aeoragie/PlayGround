namespace PlayGround.Infrastructure.Store
{
    /// <summary>
    /// Redis 작업 결과 (성공/실패/빈값)
    /// </summary>
    public class RedisResult<T>
    {
        public bool IsSuccess { get; init; }
        public bool HasValue { get; init; }
        public T? Value { get; init; }
        public Exception? Error { get; init; }

        public static RedisResult<T> Ok(T? value) => new()
        {
            IsSuccess = true,
            HasValue = value is not null,
            Value = value
        };

        public static RedisResult<T> Empty() => new()
        {
            IsSuccess = true,
            HasValue = false,
            Value = default
        };

        public static RedisResult<T> Fail() => new()
        {
            IsSuccess = false,
            HasValue = false,
        };

        public static RedisResult<T> Fail(Exception ex) => new()
        {
            IsSuccess = false,
            HasValue = false,
            Error = ex
        };
    }
}
