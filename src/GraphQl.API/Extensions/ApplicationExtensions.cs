using FluentValidation;
using GraphQl.Application.Common.Behaviors;

namespace GraphQl.API.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // MediatR Configuration
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(GraphQl.Application.Features.Products.CreateProduct.CreateProductCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(
            typeof(GraphQl.Application.Features.Products.CreateProduct.CreateProductValidator).Assembly
        );

        return services;
    }
}
