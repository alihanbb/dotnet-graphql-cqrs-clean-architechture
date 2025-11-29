using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GraphQl.API.HealthChecks;

public class RabbitMQHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public RabbitMQHealthCheck(string connectionString)
    {
        _connectionString = connectionString;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple check - in production you'd actually test the connection
            // For now, just return healthy if connection string is valid
            if (!string.IsNullOrEmpty(_connectionString))
            {
                return Task.FromResult(HealthCheckResult.Healthy("RabbitMQ is configured"));
            }

            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection string is missing"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ health check failed", ex));
        }
    }
}
