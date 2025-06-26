using Bubala.Application.Models;

namespace Bubala.Application.Services;

public interface IProductService
{///Todo: use of DTOS here
    Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdOrSlugAsync(string idOrSlung, Guid? userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetAllAsync( Guid? userId = default, CancellationToken cancellationToken = default);
    Task<Product?> UpdateAsync(Product product, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default );
}