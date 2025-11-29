using GraphQl.Domain.Events;
using GraphQl.Domain.Repositories;
using MassTransit;
using MediatR;

namespace GraphQl.Application.Features.Products.DeleteProduct;

// Command
public record DeleteProductCommand(
    string Id
) : IRequest<DeleteProductDto>;

// Command Handler
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, DeleteProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteProductCommandHandler(IProductRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<DeleteProductDto> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        // Check if product exists
        var existingProduct = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");
        }

        // Delete from MongoDB (Write Model)
        var success = await _repository.DeleteAsync(request.Id, cancellationToken);
        
        if (!success)
        {
            throw new Exception($"Failed to delete product with ID {request.Id}");
        }

        // Publish Event to RabbitMQ
        var productDeletedEvent = new ProductDeletedEvent(
            request.Id,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(productDeletedEvent, cancellationToken);

        // Return DTO
        return new DeleteProductDto(
            true,
            $"Product {request.Id} deleted successfully"
        );
    }
}
