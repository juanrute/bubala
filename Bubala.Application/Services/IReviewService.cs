using Bubala.Application.Models;
using Bubala.Application.Repositories;

namespace Bubala.Application.Services;

public interface IReviewService
{
    Task<bool> ReviewProductAsync(Guid productId, int review, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteReviewAsinc(Guid productId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductReview>> GetReviewsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}