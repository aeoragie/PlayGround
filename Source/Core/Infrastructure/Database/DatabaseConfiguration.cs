namespace PlayGround.Infrastructure.Database;

public class DatabaseConfiguration
{
    public static readonly string Section = "DatabaseConfiguration";

    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);
    public Dictionary<DatabaseTypes, DatabaseOptions> Databases { get; set; } = new();

    public record ProviderConnection(DatabaseProvider Provider, string Connection);
    public Dictionary<DatabaseTypes, ProviderConnection> ProviderConnections { get; set; } = new();

    public DatabaseOptions GetDatabaseOptions(DatabaseTypes database)
    {
        if (Databases.TryGetValue(database, out var options))
        {
            return options;
        }

        throw new InvalidOperationException($"Database configuration for {database} not found.");
    }

    public ProviderConnection GetProviderConnection(DatabaseTypes database)
    {
        if (ProviderConnections.TryGetValue(database, out var pair))
        {
            return pair;
        }
        else
        {
            var options = GetDatabaseOptions(database);
            var npair = new ProviderConnection(options.Provider, options.GetConnectionString());
            ProviderConnections[database] = npair;
            return npair;
        }
    }

    public bool HasDatabase(DatabaseTypes database)
    {
        return Databases.ContainsKey(database);
    }
}
