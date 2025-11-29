using GraphQl.Application.Common.Interfaces;
using GraphQl.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GraphQl.Application.Features.Products.Consumers;

public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    private readonly ISearchService _searchService;
    private readonly ILogger<ProductCreatedConsumer> _logger;

    public ProductCreatedConsumer(ISearchService searchService, ILogger<ProductCreatedConsumer> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        try
        {
            var @event = context.Message;

            _logger.LogInformation(
                "ProductCreatedEvent received for Product ID: {ProductId}, Name: {ProductName}",
                @event.Id,
                @event.Name
            );

            // Index product to Elasticsearch (Read Model)
            await _searchService.IndexProductAsync(
                @event.Id,
                @event.Name,
                @event.Description,
                @event.Price,
                @event.Stock,
                @event.CreatedAt,
                context.CancellationToken
            );

            _logger.LogInformation(
                "Product {ProductId} successfully indexed to Elasticsearch",
                @event.Id
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing product to Elasticsearch");
            throw;
        }
    }
}
