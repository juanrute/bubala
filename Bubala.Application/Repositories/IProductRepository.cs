using Bubala.Application.Models;
namespace Bubala.Application.Repositories;

public interface IProductRepository
{
    Task<bool> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<bool> UpdateAsync(Product product);
    Task<bool> DeleteByIdAsync(Guid id);
}