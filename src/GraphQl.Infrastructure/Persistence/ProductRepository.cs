using GraphQl.Domain.Entities;
using GraphQl.Domain.Repositories;
using MongoDB.Driver;

namespace GraphQl.Infrastructure.Persistence;

public class ProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _products;

    public ProductRepository(IMongoDatabase database, MongoDbSettings settings)
    {
        _products = database.GetCollection<Product>(settings.ProductsCollectionName);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _products.InsertOneAsync(product, cancellationToken: cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        return await _products.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _products.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(string id, Product product, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var result = await _products.ReplaceOneAsync(filter, product, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        var result = await _products.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}
