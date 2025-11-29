using GraphQl.Application.Common.Interfaces;
using GraphQl.Domain.Repositories;
using GraphQl.Infrastructure.Persistence;
using GraphQl.Infrastructure.Search;
using MongoDB.Driver;
using Nest;

namespace GraphQl.API.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()!;
        services.AddSingleton(mongoSettings);

        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(mongoSettings.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings.DatabaseName);
        });

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    public static IServiceCollection AddSearchServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var elasticSettings = configuration.GetSection("ElasticsearchSettings").Get<ElasticsearchSettings>()!;
        services.AddSingleton(elasticSettings);

        services.AddSingleton<IElasticClient>(sp =>
        {
            var settings = new ConnectionSettings(new Uri(elasticSettings.Url))
                .DefaultIndex(elasticSettings.ProductIndex);

            return new ElasticClient(settings);
        });

        services.AddScoped<ISearchService, SearchService>();

        return services;
    }
}
