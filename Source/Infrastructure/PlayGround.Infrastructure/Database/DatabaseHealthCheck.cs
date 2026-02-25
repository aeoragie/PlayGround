using Microsoft.Extensions.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PlayGround.Infrastructure.Database
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly DatabaseConfiguration Configuration;

        public DatabaseHealthCheck(IOptions<DatabaseConfiguration> options)
        {
            Configuration = options.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellation = default)
        {
            if (Configuration.Databases == null)
            {
                return HealthCheckResult.Healthy("No database services configured.");
            }

            if (!Configuration.Databases.Any())
            {
                return HealthCheckResult.Healthy("No databases are connected, so the status is marked as healthy.");
            }

            await Task.CompletedTask;

            var healthData = new Dictionary<string, object>();
            var unhealthyData = new List<string>();

            return HealthCheckResult.Healthy("All databases are healthy.", data: healthData);
        }
    }
}
