using GraphQl.Domain.Events;
using GraphQl.Domain.Repositories;
using GraphQl.Domain.Entities;
using MassTransit;
using MediatR;

namespace GraphQl.Application.Features.Products.UpdateProduct;

// Command
public record UpdateProductCommand(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int Stock
) : IRequest<UpdateProductDto>;

// Command Handler
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateProductCommandHandler(IProductRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<UpdateProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // Get existing product to preserve CreatedAt
        var existingProduct = await _repository.GetByIdAsync(request.Id, cancellationToken);
        
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");
        }

        // Update Product Entity
        var updatedProduct = new Product
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = existingProduct.CreatedAt // Preserve original creation time
        };

        // Update in MongoDB (Write Model)
        var success = await _repository.UpdateAsync(request.Id, updatedProduct, cancellationToken);
        
        if (!success)
        {
            throw new Exception($"Failed to update product with ID {request.Id}");
        }

        // Publish Event to RabbitMQ
        var productUpdatedEvent = new ProductUpdatedEvent(
            updatedProduct.Id,
            updatedProduct.Name,
            updatedProduct.Description,
            updatedProduct.Price,
            updatedProduct.Stock,
            DateTime.UtcNow
        );

        await _publishEndpoint.Publish(productUpdatedEvent, cancellationToken);

        // Return DTO
        return new UpdateProductDto(
            updatedProduct.Id,
            updatedProduct.Name,
            updatedProduct.Price,
            DateTime.UtcNow
        );
    }
}
