using GraphQl.Domain.Events;
using GraphQl.Domain.Repositories;
using GraphQl.Domain.Entities;
using MassTransit;
using MediatR;

namespace GraphQl.Application.Features.Products.CreateProduct;

// Command
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock
) : IRequest<CreateProductDto>;

// Command Handler
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductCommandHandler(IProductRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Create Product Entity
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        // Save to MongoDB (Write Model)
        var createdProduct = await _repository.AddAsync(product, cancellationToken);

        // Publish Event to RabbitMQ
        var productCreatedEvent = new ProductCreatedEvent(
            createdProduct.Id,
            createdProduct.Name,
            createdProduct.Description,
            createdProduct.Price,
            createdProduct.Stock,
            createdProduct.CreatedAt
        );

        await _publishEndpoint.Publish(productCreatedEvent, cancellationToken);

        // Return DTO
        return new CreateProductDto(
            createdProduct.Id,
            createdProduct.Name,
            createdProduct.Price,
            createdProduct.CreatedAt
        );
    }
}
