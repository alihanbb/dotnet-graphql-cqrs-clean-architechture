namespace GraphQl.Application.Features.Products.UpdateProduct;

public record UpdateProductDto(
    string Id,
    string Name,
    decimal Price,
    DateTime UpdatedAt
);
