namespace GraphQl.Application.Features.Products.SearchProducts;

public record ProductSearchDto(
    string Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    DateTime CreatedAt
);
