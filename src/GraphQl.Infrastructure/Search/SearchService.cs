using Nest;
using GraphQl.Application.Common.Interfaces;
using GraphQl.Application.Features.Products.SearchProducts;

namespace GraphQl.Infrastructure.Search;

public class SearchService : ISearchService
{
    private readonly IElasticClient _client;
    private readonly ElasticsearchSettings _settings;

    public SearchService(IElasticClient client, ElasticsearchSettings settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task IndexProductAsync(
        string id,
        string name,
        string description,
        decimal price,
        int stock,
        DateTime createdAt,
        CancellationToken cancellationToken = default)
    {
        var productDocument = new ProductDocument
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CreatedAt = createdAt
        };

        var response = await _client.IndexAsync(productDocument, idx => idx
            .Index(_settings.ProductIndex)
            .Id(id), 
            cancellationToken);
        
        if (!response.IsValid)
        {
            throw new Exception($"Failed to index product: {response.DebugInformation}");
        }
    }

    public async Task UpdateProductAsync(
        string id,
        string name,
        string description,
        decimal price,
        int stock,
        CancellationToken cancellationToken = default)
    {
        var productDocument = new ProductDocument
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            Stock = stock,
            CreatedAt = DateTime.UtcNow // This should ideally come from the existing document
        };

        // Update is the same as index with the same ID in Elasticsearch
        var response = await _client.IndexAsync(productDocument, idx => idx
            .Index(_settings.ProductIndex)
            .Id(id), 
            cancellationToken);
        
        if (!response.IsValid)
        {
            throw new Exception($"Failed to update product: {response.DebugInformation}");
        }
    }

    public async Task<List<ProductSearchDto>> SearchProductsAsync(
        string? searchTerm,
        int pageSize,
        int pageNumber,
        CancellationToken cancellationToken = default)
    {
        var from = (pageNumber - 1) * pageSize;

        ISearchResponse<ProductDocument> searchResponse;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            searchResponse = await _client.SearchAsync<ProductDocument>(s => s
                .Index(_settings.ProductIndex)
                .From(from)
                .Size(pageSize)
                .Sort(sort => sort.Descending(f => f.CreatedAt)),
                cancellationToken
            );
        }
        else
        {
            searchResponse = await _client.SearchAsync<ProductDocument>(s => s
                .Index(_settings.ProductIndex)
                .From(from)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(searchTerm)
                        .Fields(f => f
                            .Field(p => p.Name, 3)
                            .Field(p => p.Description)
                        )
                    )
                )
                .Sort(sort => sort.Descending(f => f.CreatedAt)),
                cancellationToken
            );
        }

        if (!searchResponse.IsValid)
        {
            return new List<ProductSearchDto>();
        }

        return searchResponse.Documents.Select(doc => new ProductSearchDto(
            doc.Id,
            doc.Name,
            doc.Description,
            doc.Price,
            doc.Stock,
            doc.CreatedAt
        )).ToList();
    }

    public async Task<ProductSearchDto?> GetProductByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync<ProductDocument>(id, idx => idx
            .Index(_settings.ProductIndex), 
            cancellationToken);

        if (!response.IsValid || response.Source == null)
        {
            return null;
        }

        var doc = response.Source;
        return new ProductSearchDto(doc.Id, doc.Name, doc.Description, doc.Price, doc.Stock, doc.CreatedAt);
    }

    public async Task DeleteProductAsync(string id, CancellationToken cancellationToken = default)
    {
        await _client.DeleteAsync<ProductDocument>(id, idx => idx.Index(_settings.ProductIndex), cancellationToken);
    }
}

// Internal document model for Elasticsearch
internal class ProductDocument
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
}
