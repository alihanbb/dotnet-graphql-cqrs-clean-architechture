using GraphQl.Application.Features.Products.SearchProducts;

namespace GraphQl.Application.Common.Interfaces;

public interface ISearchService
{
    Task IndexProductAsync(
        string id,
        string name,
        string description,
        decimal price,
        int stock,
        DateTime createdAt,
        CancellationToken cancellationToken = default
    );

    Task UpdateProductAsync(
        string id,
        string name,
        string description,
        decimal price,
        int stock,
        CancellationToken cancellationToken = default
    );

    Task<List<ProductSearchDto>> SearchProductsAsync(
        string? searchTerm,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default
    );

    Task<ProductSearchDto?> GetProductByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task DeleteProductAsync(
        string id,
        CancellationToken cancellationToken = default
    );
}
