using GraphQl.Domain.Entities;

namespace GraphQl.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(string id, Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
