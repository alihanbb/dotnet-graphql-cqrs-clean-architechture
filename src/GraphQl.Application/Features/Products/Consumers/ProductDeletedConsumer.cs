using GraphQl.Application.Common.Interfaces;
using GraphQl.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GraphQl.Application.Features.Products.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeletedEvent>
{
    private readonly ISearchService _searchService;
    private readonly ILogger<ProductDeletedConsumer> _logger;

    public ProductDeletedConsumer(ISearchService searchService, ILogger<ProductDeletedConsumer> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        try
        {
            var @event = context.Message;

            _logger.LogInformation(
                "ProductDeletedEvent received for Product ID: {ProductId}",
                @event.Id
            );

            // Delete product from Elasticsearch (Read Model)
            await _searchService.DeleteProductAsync(
                @event.Id,
                context.CancellationToken
            );

            _logger.LogInformation(
                "Product {ProductId} successfully deleted from Elasticsearch",
                @event.Id
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product from Elasticsearch");
            throw;
        }
    }
}
