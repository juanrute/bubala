using Bubala.Application.Models;

namespace Bubala.Application.Repositories;

public interface IReviewRepository
{
    Task<bool> ReviewProduct(Guid productId, int rating, Guid userId, CancellationToken cancellationToken = default);
    Task<float?> GetReviewAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<(float? Review, int? userReview)> GetReviewAsync(Guid productId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteReviewAsinc(Guid productId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductReview>> GetReviewsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}