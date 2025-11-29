using GraphQl.Application.Common.Interfaces;
using MediatR;

namespace GraphQl.Application.Features.Products.SearchProducts;

// Query
public record SearchProductsQuery(
    string? SearchTerm = null,
    int PageSize = 10,
    int PageNumber = 1
) : IRequest<List<ProductSearchDto>>;

// Query Handler
public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductSearchDto>>
{
    private readonly ISearchService _searchService;

    public SearchProductsQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<List<ProductSearchDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        // Query Elasticsearch (Read Model)
        var products = await _searchService.SearchProductsAsync(
            request.SearchTerm,
            request.PageSize,
            request.PageNumber,
            cancellationToken
        );

        return products;
    }
}
