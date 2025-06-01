using Bubala.Application.Models;

namespace Bubala.Application.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    public Task<bool> CreateAsync(Product product)
    {
        _products.Add(product);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        var removedCount = _products.RemoveAll(p => p.Id == id);
        var isProductRemoved = removedCount < 0;
        return Task.FromResult(isProductRemoved);
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        return Task.FromResult(_products.AsEnumerable());
    }

    public Task<Product?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    public Task<Product?> GetBySlugAsync(string slug)
    {
        return Task.FromResult(_products.FirstOrDefault(p => p.Slug == slug));
    }

    public Task<bool> UpdateAsync(Product product)
    {
        var productIndexToUpdate = _products.FindIndex(p => p.Id == product.Id);
        if (productIndexToUpdate == -1)
        {
            return Task.FromResult(false);
        }
        _products[productIndexToUpdate] = product;
        return Task.FromResult(true);
    }
}