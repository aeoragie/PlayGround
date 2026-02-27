using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Dapper;
using NLog;
using PlayGround.Shared.Result;

namespace PlayGround.Infrastructure.Database.Base;

public abstract class RepositoryBase
{
    protected readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly DatabaseConfiguration Configuration;

    public abstract DatabaseTypes Database { get; }

    protected DatabaseOptions Options => Configuration.GetDatabaseOptions(Database);

    protected RepositoryBase(IOptions<DatabaseConfiguration> options)
    {
        Configuration = options.Value;
    }

    #region Connection Management

    public DbConnection CreateConnection()
    {
        return CreateConnection(Database);
    }

    public DbConnection CreateConnection(DatabaseTypes databaseType)
    {
        var pair = Configuration.GetProviderConnection(databaseType);
        return pair.Provider switch
        {
            DatabaseProvider.SqlServer => new SqlConnection(pair.Connection),
            DatabaseProvider.MySql => throw new NotImplementedException("MySQL provider is not implemented yet."),
            DatabaseProvider.PostgreSql => throw new NotImplementedException("PostgreSQL provider is not implemented yet."),
            _ => throw new NotSupportedException($"Database provider {pair.Provider} is not supported.")
        };
    }

    protected virtual async Task<DbConnection> OpenConnectionAsync(CancellationToken cancellation = default)
    {
        var connection = CreateConnection();
        await connection.OpenAsync(cancellation);
        Logger.Trace("Connection opened for {Database}", Database);
        return connection;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);
            stopwatch.Stop();
            Logger.Info("Connection test successful for {Database} in {ElapsedMs}ms", Database, stopwatch.ElapsedMilliseconds);
            return true;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Connection test failed for {Database} after {ElapsedMs}ms", Database, stopwatch.ElapsedMilliseconds);
            return false;
        }
    }

    #endregion

    #region Single Query

    public async Task<Result<TRow>> QuerySingleOrDefaultAsync<TRow>(
        string sql, object? parameters = null, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);
            var result = await connection.QuerySingleOrDefaultAsync<TRow>(sql, parameters, commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();

            if (result == null)
            {
                Logger.Debug("Query returned no result in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
                return Result<TRow>.Error(ErrorCode.NotFound, "No data found");
            }

            Logger.Debug("Query executed in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<TRow>.Success(result);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<TRow>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Query failed in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<TRow>.FromException(ex);
        }
    }

    public async Task<Result<TRow>> ProcedureSingleOrDefaultAsync<TRow>(
        ProcedureBase procedure, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);
            var result = await connection.QuerySingleOrDefaultAsync<TRow>(
                procedure.Procedure,
                procedure.BuildParameters(),
                commandType: CommandType.StoredProcedure, commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();

            if (result == null)
            {
                Logger.Debug("Procedure returned no result in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
                return Result<TRow>.Error(ErrorCode.NotFound, "No data found");
            }

            Logger.Debug("Procedure executed in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<TRow>.Success(result);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<TRow>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Procedure failed in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<TRow>.FromException(ex);
        }
    }

    #endregion

    #region Multiple Query

    public async Task<Result<IEnumerable<TRow>>> QueryAsync<TRow>(
        string sql, object? parameters = null, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);
            var result = await connection.QueryAsync<TRow>(sql, parameters, commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();

            var count = result.TryGetNonEnumeratedCount(out var c) ? c : -1;
            Logger.Debug("Query returned {Count} rows in {ElapsedMs}ms: {Sql}", count, stopwatch.ElapsedMilliseconds, TruncateSql(sql));

            return Result<IEnumerable<TRow>>.Success(result);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<IEnumerable<TRow>>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Query failed in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<IEnumerable<TRow>>.FromException(ex);
        }
    }

    public async Task<Result<IEnumerable<TRow>>> ProcedureAsync<TRow>(
        ProcedureBase procedure, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);
            var result = await connection.QueryAsync<TRow>(
                procedure.Procedure,
                procedure.BuildParameters(),
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();
            var count = result.TryGetNonEnumeratedCount(out var c) ? c : -1;
            Logger.Debug("Procedure returned {Count} rows in {ElapsedMs}ms: {Procedure}", count, stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<IEnumerable<TRow>>.Success(result);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<IEnumerable<TRow>>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Procedure failed in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<IEnumerable<TRow>>.FromException(ex);
        }
    }

    public async Task<Result<SqlMapper.GridReader>> ProcedureMultipleAsync(
        ProcedureBase procedure, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var connection = await OpenConnectionAsync(cancellation);
            var reader = await connection.QueryMultipleAsync(
                procedure.Procedure,
                procedure.BuildParameters(),
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();
            Logger.Debug("Procedure multiple query executed in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<SqlMapper.GridReader>.Success(reader);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<SqlMapper.GridReader>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Procedure multiple query failed in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<SqlMapper.GridReader>.FromException(ex);
        }
    }

    #endregion

    #region Execute (Insert/Update/Delete)

    public async Task<Result<int>> ExecuteAsync(
        string sql, object? parameters = null, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);
            var affectedRows = await connection.ExecuteAsync(sql, parameters, commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();
            Logger.Debug("Execute affected {AffectedRows} rows in {ElapsedMs}ms: {Sql}", affectedRows, stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<int>.Success(affectedRows);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<int>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Execute failed in {ElapsedMs}ms: {Sql}", stopwatch.ElapsedMilliseconds, TruncateSql(sql));
            return Result<int>.FromException(ex);
        }
    }

    public async Task<Result<int>> ProcedureExecuteAsync(
        ProcedureBase procedure, int? commandTimeout = null, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await using var connection = await OpenConnectionAsync(cancellation);

            var affectedRows = await connection.ExecuteAsync(
                procedure.Procedure,
                procedure.BuildParameters(),
                commandType: CommandType.StoredProcedure,
                commandTimeout: commandTimeout ?? Options.CommandTimeout);

            stopwatch.Stop();
            Logger.Debug("Procedure affected {AffectedRows} rows in {ElapsedMs}ms: {Procedure}", affectedRows, stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<int>.Success(affectedRows);
        }
        catch (SqlException ex) when (IsTransientError(ex))
        {
            stopwatch.Stop();
            Logger.Warn(ex, "Transient SQL error in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<int>.Error(ErrorCode.TransactionFailed, ex.Message);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Logger.Error(ex, "Procedure execute failed in {ElapsedMs}ms: {Procedure}", stopwatch.ElapsedMilliseconds, procedure.Procedure);
            return Result<int>.FromException(ex);
        }
    }

    #endregion

    #region Transaction Support

    public async Task<Result<TResult>> ExecuteInTransactionAsync<TResult>(
        Func<DbConnection, DbTransaction, Task<TResult>> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellation = default)
    {
        var stopwatch = Stopwatch.StartNew();
        await using var connection = await OpenConnectionAsync(cancellation);
        await using var transaction = await connection.BeginTransactionAsync(isolationLevel, cancellation);

        try
        {
            var result = await operation(connection, transaction);
            await transaction.CommitAsync(cancellation);

            stopwatch.Stop();
            Logger.Debug("Transaction committed in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return Result<TResult>.Success(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await transaction.RollbackAsync(cancellation);
            Logger.Error(ex, "Transaction rolled back after {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return Result<TResult>.FromException(ex);
        }
    }

    public async Task<Result<int>> ExecuteInTransactionAsync(
        Func<DbConnection, DbTransaction, Task<int>> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellation = default)
    {
        return await ExecuteInTransactionAsync<int>(operation, isolationLevel, cancellation);
    }

    #endregion

    #region Retry Support

    public async Task<Result<TResult>> ExecuteWithRetryAsync<TResult>(
        Func<Task<Result<TResult>>> operation, int? maxRetries = null, CancellationToken cancellation = default)
    {
        var retryCount = maxRetries ?? Options.MaxRetryCount;
        var attempt = 0;
        while (true)
        {
            attempt++;

            var result = await operation();
            if (result.IsSuccess)
            {
                return result;
            }

            if (result.ResultData.DetailCode != ErrorCode.TransactionFailed || attempt >= retryCount)
            {
                if (attempt > 1)
                {
                    Logger.Warn("Operation failed after {Attempts} attempts", attempt);
                }
                return result;
            }

            var delay = Options.RetryDelayMilliseconds * (int)Math.Pow(2, attempt - 1);
            Logger.Debug("Retry attempt {Attempt}/{MaxRetries} after {Delay}ms", attempt, retryCount, delay);
            await Task.Delay(delay, cancellation);
        }
    }

    #endregion

    #region Helper Methods

    private static bool IsTransientError(SqlException ex)
    {
        // SQL Server transient error codes
        int[] transientErrorNumbers =
        [
            -2,     // Timeout
            20,     // The instance of SQL Server does not support encryption
            64,     // Connection was successfully established but then an error occurred
            233,    // Connection initialization error
            10053,  // Connection forcibly closed
            10054,  // Connection reset by peer
            10060,  // Connection timeout
            40197,  // Service error processing request
            40501,  // Service busy
            40613,  // Database unavailable
            49918,  // Cannot process request (not enough resources)
            49919,  // Cannot process create/update request (too many operations)
            49920   // Cannot process request (too many operations)
        ];

        return transientErrorNumbers.Contains(ex.Number);
    }

    private static string TruncateSql(string sql, int maxLength = 100)
    {
        if (string.IsNullOrEmpty(sql))
        {
            return string.Empty;
        }

        var normalized = sql.Replace("\r\n", " ").Replace("\n", " ").Trim();
        return normalized.Length <= maxLength ? normalized : string.Concat(normalized.AsSpan(0, maxLength), "...");
    }

    #endregion
}
