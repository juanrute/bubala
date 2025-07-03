
using Bubala.Application.Database;
using Bubala.Application.Models;
using Dapper;

namespace Bubala.Application.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ReviewRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> DeleteReviewAsinc(Guid productId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken); 
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from reviews
            where productid = @productid
            and userid = @userId
        """, new { userId, productId }, cancellationToken: cancellationToken
        )
        );
        return result > 0;
    }

    public async Task<float?> GetReviewAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken); 
        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select 
                round(avg(r.review),1)
            from reviews r 
            where r.productid = @productId
        """, new { productId }, cancellationToken: cancellationToken));
    }

    public async Task<(float? Review, int? userReview)> GetReviewAsync(Guid productId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken); 
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select 
                round(avg(review),1),
                (select review from reviews where userid = @userid and productid = @productId limit 1)
            from reviews
            where productid = @productId
        """, new { productId, userId }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<ProductReview>> GetReviewsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        return await connection.QueryAsync<ProductReview>(new CommandDefinition("""
            select r.review, r.productid, p.slug
            from reviews r 
            inner join  product p on r.productid = p.id
            where userid = @userId
        """, new { userId }, cancellationToken: cancellationToken)
        );
    }

    public async Task<bool> ReviewProduct(Guid productId, int review, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into reviews(userid,productid,review) 
            values (@userId,@productid,@review)
            on conflict (userid,productid) do update 
                set review = @review
        """, new { userId, productId, review }, cancellationToken: cancellationToken
        )
        );
        return result > 0;
    }
}