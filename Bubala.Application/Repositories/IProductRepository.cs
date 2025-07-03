using Bubala.Application.Models;
namespace Bubala.Application.Repositories;

public interface IProductRepository
{
    Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid productId, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<Product?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync( Guid? userId = default, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid productId, CancellationToken cancellationToken = default);
}