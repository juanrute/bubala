
using Bubala.Application.Models;
using Bubala.Application.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Bubala.Application.Services;

public class ReviewService(IReviewRepository reviewRepository,IProductRepository productRepository) : IReviewService
{
    public async Task<bool> DeleteReviewAsinc(Guid productId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await reviewRepository.DeleteReviewAsinc(productId, userId, cancellationToken);
    }

    public async Task<IEnumerable<ProductReview>> GetReviewsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await reviewRepository.GetReviewsForUserAsync(userId, cancellationToken);
    }

    public async Task<bool> ReviewProductAsync(Guid productId, int review, Guid userId, CancellationToken cancellationToken = default)
    {
        if (review is <= 0 or > 5)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure{
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            });
        }
        var productExist = await productRepository.ExistsByIdAsync(productId, cancellationToken);
        if (!productExist)
        {
            return false;
        }
        return await reviewRepository.ReviewProduct(productId,review,userId,cancellationToken);
    }
}