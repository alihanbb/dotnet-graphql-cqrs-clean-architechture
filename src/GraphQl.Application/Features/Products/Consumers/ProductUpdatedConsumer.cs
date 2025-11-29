using GraphQl.Application.Common.Interfaces;
using GraphQl.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GraphQl.Application.Features.Products.Consumers;

public class ProductUpdatedConsumer : IConsumer<ProductUpdatedEvent>
{
    private readonly ISearchService _searchService;
    private readonly ILogger<ProductUpdatedConsumer> _logger;

    public ProductUpdatedConsumer(ISearchService searchService, ILogger<ProductUpdatedConsumer> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        try
        {
            var @event = context.Message;

            _logger.LogInformation(
                "ProductUpdatedEvent received for Product ID: {ProductId}, Name: {ProductName}",
                @event.Id,
                @event.Name
            );

            // Update product in Elasticsearch (Read Model)
            await _searchService.UpdateProductAsync(
                @event.Id,
                @event.Name,
                @event.Description,
                @event.Price,
                @event.Stock,
                context.CancellationToken
            );

            _logger.LogInformation(
                "Product {ProductId} successfully updated in Elasticsearch",
                @event.Id
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product in Elasticsearch");
            throw;
        }
    }
}
