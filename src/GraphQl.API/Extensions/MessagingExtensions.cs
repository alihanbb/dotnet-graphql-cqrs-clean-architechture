using GraphQl.Application.Features.Products.Consumers;
using GraphQl.Domain.Constants;
using MassTransit;

namespace GraphQl.API.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddMessagingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitMqHost = configuration["RabbitMqSettings:Host"]!;
        var rabbitMqUser = configuration["RabbitMqSettings:Username"]!;
        var rabbitMqPass = configuration["RabbitMqSettings:Password"]!;

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<ProductUpdatedConsumer>();
            x.AddConsumer<ProductDeletedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost, h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
                });

                cfg.ReceiveEndpoint(QueueNames.ProductCreatedQueue, e =>
                {
                    e.ConfigureConsumer<ProductCreatedConsumer>(context);
                    
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30)
                    ));

                    e.PrefetchCount = 16;
                    e.UseInMemoryOutbox();
                });

                cfg.ReceiveEndpoint(QueueNames.ProductUpdatedQueue, e =>
                {
                    e.ConfigureConsumer<ProductUpdatedConsumer>(context);
                    
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30)
                    ));

                    e.PrefetchCount = 16;
                    e.UseInMemoryOutbox();
                });

                cfg.ReceiveEndpoint(QueueNames.ProductDeletedQueue, e =>
                {
                    e.ConfigureConsumer<ProductDeletedConsumer>(context);
                    
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(30)
                    ));

                    e.PrefetchCount = 16;
                    e.UseInMemoryOutbox();
                });
            });
        });

        return services;
    }
}
