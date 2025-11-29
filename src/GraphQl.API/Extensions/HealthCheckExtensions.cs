using GraphQl.API.HealthChecks;

namespace GraphQl.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMqSettings:Host"]!;
        var rabbitMqUser = configuration["RabbitMqSettings:Username"]!;
        var rabbitMqPass = configuration["RabbitMqSettings:Password"]!;
        var connectionString = $"amqp://{rabbitMqUser}:{rabbitMqPass}@{rabbitMqHost}:5672";

        services.AddHealthChecks()
            .AddCheck("rabbitmq", new RabbitMQHealthCheck(connectionString), tags: new[] { "messaging" });

        return services;
    }
}
