namespace Bubala.Application.Repositories;

public interface IReviewRepository
{
    Task<float?> GetReviewAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<(float? Review,int? userReview)> GetReviewAsync(Guid productId, Guid userId, CancellationToken cancellationToken = default);
}