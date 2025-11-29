using GraphQl.API.GraphQL.Products;

namespace GraphQl.API.Extensions;

public static class GraphQLExtensions
{
    public static IServiceCollection AddGraphQLServices(
        this IServiceCollection services)
    {
        services
            .AddGraphQLServer()
            .AddQueryType<ProductQueries>()
            .AddMutationType<ProductMutations>();

        return services;
    }
}
