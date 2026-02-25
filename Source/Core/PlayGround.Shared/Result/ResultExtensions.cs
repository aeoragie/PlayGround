namespace PlayGround.Shared.Result;

public static class ResultExtensions {
    public static HttpResponseInfo ToHttpResponse<T>(this Result<T> result) {
        return new HttpResponseInfo {
            StatusCode = result.ResultData.DetailCode.ToHttpStatusCode(),
            IsSuccess = result.IsSuccess,
            Message = result.ResultData.DetailCode.GetUserFriendlyMessage(result.Message),
            Code = result.ResultData.DetailCode.Name,
            Value = result.Value,
            Details = result.ResultData.Details,
            Timestamp = DateTime.Now
        };
    }

    public static HttpResponseInfo ToHttpResponse(this Result result) {
        return new HttpResponseInfo {
            StatusCode = result.ResultData.DetailCode.ToHttpStatusCode(),
            IsSuccess = result.IsSuccess,
            Message = result.ResultData.DetailCode.GetUserFriendlyMessage(result.Message),
            Code = result.ResultData.DetailCode.Name,
            Value = null,
            Details = result.ResultData.Details,
            Timestamp = DateTime.Now
        };
    }

    public static LogInfo ToLogInfo<T>(this Result<T> result, string? operationName = null) {
        return new LogInfo {
            Level = result.ResultData.DetailCode.GetLogLevel(),
            Category = result.ResultData.DetailCode.GetMetricCategory(),
            Code = result.ResultData.DetailCode.Name,
            Message = result.Message,
            Details = result.ResultData.Details,
            OperationName = operationName,
            IsSuccess = result.IsSuccess,
            Priority = result.ResultData.DetailCode.GetPriority(),
            RequiresNotification = result.ResultData.DetailCode.RequiresNotification(),
            Timestamp = DateTime.Now
        };
    }

    public static LogInfo ToLogInfo(this Result result, string? operationName = null) {
        return new LogInfo {
            Level = result.ResultData.DetailCode.GetLogLevel(),
            Category = result.ResultData.DetailCode.GetMetricCategory(),
            Code = result.ResultData.DetailCode.Name,
            Message = result.Message,
            Details = result.ResultData.Details,
            OperationName = operationName,
            IsSuccess = result.IsSuccess,
            Priority = result.ResultData.DetailCode.GetPriority(),
            RequiresNotification = result.ResultData.DetailCode.RequiresNotification(),
            Timestamp = DateTime.Now
        };
    }

    public static MetricInfo ToMetricInfo<T>(this Result<T> result, string operationName, TimeSpan? duration = null) {
        return new MetricInfo {
            OperationName = operationName,
            Category = result.ResultData.DetailCode.GetMetricCategory(),
            Code = result.ResultData.DetailCode.Name,
            IsSuccess = result.IsSuccess,
            IsRetryable = result.ResultData.DetailCode.IsRetryable(),
            Priority = result.ResultData.DetailCode.GetPriority(),
            Duration = duration ?? TimeSpan.Zero,
            Timestamp = DateTime.Now
        };
    }

    public static async Task<Result<T>> OnSuccessAsync<T>(this Result<T> result, Func<T, Task> action) {
        if (result.IsSuccess) {
            await action(result.Value!);
        }
        return result;
    }

    public static async Task<Result<T>> OnErrorAsync<T>(this Result<T> result, Func<ResultInfo, Task> action) {
        if (result.IsError) {
            await action(result.ResultData);
        }
        return result;
    }

    public static async Task<Result<T>> OnErrorCodeAsync<T>(this Result<T> result, ErrorCode errorCode, Func<ResultInfo, Task> action) {
        if (result.IsError && result.ResultData.DetailCode == errorCode) {
            await action(result.ResultData);
        }
        return result;
    }

    public static bool IsRetryable<T>(this Result<T> result) =>
        result.IsError && result.ResultData.DetailCode.IsRetryable();

    public static bool IsUserFriendly<T>(this Result<T> result) =>
        result.ResultData.DetailCode.IsUserFriendly();

    public static bool RequiresNotification<T>(this Result<T> result) =>
        result.ResultData.DetailCode.RequiresNotification();

    public static Result<TNew> MapWhenValue<T, TNew>(this Result<T> result, Func<T, TNew> mapper, TNew defaultValue = default!) {
        if (result.IsSuccess && result.Value != null) {
            try {
                var newValue = mapper(result.Value);
                return Result<TNew>.Success(newValue);
            }
            catch (Exception ex) {
                return Result<TNew>.FromException(ex);
            }
        }

        if (result.IsWarning && result.Value != null) {
            try {
                var newValue = mapper(result.Value);
                return Result<TNew>.Warning(newValue, (WarningCode)result.ResultData.DetailCode, result.Message, result.ResultData.Details);
            }
            catch (Exception ex) {
                return Result<TNew>.FromException(ex);
            }
        }

        if (result.IsInformation && result.Value != null) {
            try {
                var newValue = mapper(result.Value);
                return Result<TNew>.Information(newValue, (InformationCode)result.ResultData.DetailCode, result.Message, result.ResultData.Details);
            }
            catch (Exception ex) {
                return Result<TNew>.FromException(ex);
            }
        }

        return Result<TNew>.Failure(result.ResultData);
    }

    public static Result<T[]> CombineAll<T>(params Result<T>[] results) {
        var errors = results.Where(r => r.IsError).ToList();
        if (errors.Any()) {
            var firstError = errors.First();
            return Result<T[]>.Failure(firstError.ResultData);
        }

        var warnings = results.Where(r => r.IsWarning).ToList();
        if (warnings.Any()) {
            var values = results.Select(r => r.Value!).ToArray();
            var firstWarning = warnings.First();
            return Result<T[]>.Warning(values, (WarningCode)firstWarning.ResultData.DetailCode,
                $"Combined with {warnings.Count} warnings", firstWarning.ResultData.Details);
        }

        var successValues = results.Select(r => r.Value!).ToArray();
        return Result<T[]>.Success(successValues);
    }

    public static Result<T> CombineAny<T>(params Result<T>[] results) {
        var successes = results.Where(r => r.IsSuccess).ToList();
        if (successes.Any()) {
            return successes.First();
        }

        var warnings = results.Where(r => r.IsWarning).ToList();
        if (warnings.Any()) {
            return warnings.First();
        }

        return results.First();
    }
}
