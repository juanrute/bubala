using System.Reflection.Metadata;
using Bubala.Application.Models;
using Bubala.Application.Repositories;
using FluentValidation;

namespace Bubala.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Product> _productValidator;
    private readonly IReviewRepository _reviewRepository;

    public ProductService(
        IProductRepository productRepository,
        IValidator<Product> productValidator,
        IReviewRepository reviewRepository)
    {
        _productValidator = productValidator;
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
    }
    public async Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _productValidator.ValidateAndThrowAsync(product, cancellationToken);
        return await _productRepository.CreateAsync(product, cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _productRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return await _productRepository.GetAllAsync(userId, cancellationToken);
    }

    public async Task<Product?> UpdateAsync(Product product, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        await _productValidator.ValidateAndThrowAsync(product,cancellationToken);
        var productExists = await _productRepository.ExistsByIdAsync(product.Id, cancellationToken);
        if (!productExists)
        {
            return null;
        }
        await _productRepository.UpdateAsync(product, cancellationToken);

        if (!userId.HasValue)
        {
            var review = await _reviewRepository.GetReviewAsync(product.Id, cancellationToken);
            product.Review = review;
            return product;
        }
        var reviews = await _reviewRepository.GetReviewAsync(product.Id, userId.Value, cancellationToken);
        product.Review = reviews.Review;
        product.UserReview = reviews.userReview;
        return product;
    }

    public async Task<Product?> GetByIdOrSlugAsync(string idOrSlug, Guid? userId, CancellationToken cancellationToken = default)
    {
        var product = Guid.TryParse(idOrSlug, out Guid productId) ?
            await _productRepository.GetByIdAsync(productId, userId, cancellationToken) :
            await _productRepository.GetBySlugAsync(idOrSlug, userId, cancellationToken);
        return product;
    }
}