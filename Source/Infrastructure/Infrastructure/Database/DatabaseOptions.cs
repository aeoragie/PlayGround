using System.Text.RegularExpressions;

namespace PlayGround.Infrastructure.Database;

public class ConnectionPoolOptions
{
    public bool Enable { get; set; } = true;
    public int MaxPoolSize { get; set; } = 100;
    public int MinPoolSize { get; set; } = 5;
    public int ConnectionLifetime { get; set; } = 300;
}

public class DatabaseOptions
{
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableRetryOnFailure { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelay { get; set; } = 30;
    public int RetryDelayMilliseconds { get; set; } = 100;
    public ConnectionPoolOptions PoolSetting { get; set; } = new();

    public string GetConnectionString()
    {
        var connStr = ConnectionString;
        if (Provider == DatabaseProvider.SqlServer)
        {
            connStr = AddParameter(connStr, "Pooling", PoolSetting.Enable);
            connStr = AddParameter(connStr, "Min Pool Size", PoolSetting.MinPoolSize);
            connStr = AddParameter(connStr, "Max Pool Size", PoolSetting.MaxPoolSize);
            connStr = AddParameter(connStr, "Connection Lifetime", PoolSetting.ConnectionLifetime);
            connStr = AddParameter(connStr, "Connect Timeout", CommandTimeout);
            connStr = AddParameter(connStr, "Command Timeout", CommandTimeout);
            if (EnableRetryOnFailure)
            {
                connStr = AddParameter(connStr, "ConnectRetryCount", MaxRetryCount);
                connStr = AddParameter(connStr, "ConnectRetryInterval", MaxRetryDelay / MaxRetryCount);
            }
        }
        else if (Provider == DatabaseProvider.MySql)
        {
            connStr = AddParameter(connStr, "Pooling", PoolSetting.Enable);
            connStr = AddParameter(connStr, "MinimumPoolSize", PoolSetting.MinPoolSize);
            connStr = AddParameter(connStr, "MaximumPoolSize", PoolSetting.MaxPoolSize);
            connStr = AddParameter(connStr, "ConnectionLifeTime", PoolSetting.ConnectionLifetime);
            connStr = AddParameter(connStr, "ConnectionTimeout", CommandTimeout);
            connStr = AddParameter(connStr, "DefaultCommandTimeout", CommandTimeout);
        }
        else if (Provider == DatabaseProvider.PostgreSql)
        {
            connStr = AddParameter(connStr, "Pooling", PoolSetting.Enable);
            connStr = AddParameter(connStr, "MinPoolSize", PoolSetting.MinPoolSize);
            connStr = AddParameter(connStr, "MaxPoolSize", PoolSetting.MaxPoolSize);
            connStr = AddParameter(connStr, "ConnectionLifeTime", PoolSetting.ConnectionLifetime);
            connStr = AddParameter(connStr, "Timeout", CommandTimeout);
            connStr = AddParameter(connStr, "CommandTimeout", CommandTimeout);
        }

        return connStr;
    }

    private string AddParameter(string connstr, string key, bool value)
    {
        return AddParameter(connstr, key, $"{value}");
    }

    private string AddParameter(string connstr, string key, int value)
    {
        return AddParameter(connstr, key, $"{value}");
    }

    private string AddParameter(string connstr, string key, string value)
    {
        var pattern = $@"{key}\s*=\s*[^;]+";
        if (Regex.IsMatch(connstr, pattern, RegexOptions.IgnoreCase))
        {
            return Regex.Replace(
                connstr,
                pattern,
                $"{key}={value}",
                RegexOptions.IgnoreCase
            );
        }

        if (!connstr.TrimEnd().EndsWith(";"))
        {
            connstr += ";";
        }
        return connstr + $"{key}={value};";
    }
}
