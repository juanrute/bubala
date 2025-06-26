
using Bubala.Application.Database;
using Dapper;

namespace Bubala.Application.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ReviewRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    public async Task<float?> GetReviewAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken); 
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select 
                round(avg(r.review),1) as review
            from reviews r 
            where r.movieid = @productId
        """, new { productId }, cancellationToken: cancellationToken));
    }

    public async Task<(float? Review, int? userReview)> GetReviewAsync(Guid productId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken); 
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select 
                round(avg(review),1) as review,
                (select review from reviews where userid = @userid and productid = @productId limit 1) as userReview
            from reviews
            where movieid = @productId
        """, new { productId, userId }, cancellationToken: cancellationToken));
    }
}