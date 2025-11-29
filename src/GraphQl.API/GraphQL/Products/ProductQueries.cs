using GraphQl.Application.Features.Products.SearchProducts;
using MediatR;

namespace GraphQl.API.GraphQL.Products;

public class ProductQueries
{
    public async Task<List<ProductSearchDto>> SearchProducts(
        string? searchTerm,
        int pageSize = 10,
        int pageNumber = 1,
        [Service] IMediator mediator = null!,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchProductsQuery(searchTerm, pageSize, pageNumber);
        return await mediator.Send(query, cancellationToken);
    }

    public async Task<ProductSearchDto?> GetProductById(
        string id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // For simplicity, we can create a dedicated query, but for now using search service directly
        var query = new SearchProductsQuery(null, 1000, 1);
        var products = await mediator.Send(query, cancellationToken);
        return products.FirstOrDefault(p => p.Id == id);
    }
}
