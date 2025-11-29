namespace GraphQl.Application.Features.Products.CreateProduct;

public record CreateProductDto(
    string Id,
    string Name,
    decimal Price,
    DateTime CreatedAt
);
