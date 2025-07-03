using System.Reflection.Metadata;
using System.Transactions;
using Bubala.Application.Database;
using Bubala.Application.Models;
using Dapper;

namespace Bubala.Application.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ProductRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into product (id, slug, name) values (@Id, @Slug, @Name)
        """, product, cancellationToken: cancellationToken));
        if (result > 0)
        {
            foreach (var fruitType in product.FruitType)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into fruit_type (id, type_name) Values (@Id, @FruitType);
                """, new { Id = product.Id, FruitType = product.FruitType }, cancellationToken: cancellationToken));    

            }
        }
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from product where id=@productId
        """,new { productId }, cancellationToken: cancellationToken));
        if (result > 0)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
            delete from fruit_type where id = @productId
            """, new { productId }, cancellationToken: cancellationToken));
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from product where id=@productId
        """,new { productId }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Product>> GetAllAsync(Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var result = await connection.QueryAsync(new CommandDefinition("""
            select 
                p.*, 
                string_agg(distinct ft.type_name,',') as types,
                round(avg(r.review),1) as review,
                myr.review as userreview
            from product p 
                left join fruit_type ft on p.id = ft.id
                left join reviews r on p.id = r.productid
                left join reviews myr on p.id = myr.productid 
                    and myr.userid = @userId
            group by p.id,userreview
        """, new { userId }, cancellationToken: cancellationToken));

        return result.Select(x => new Product
        {
            Id = x.id,
            Name = x.name,
            FruitType = Enumerable.ToList(x.types.Split(',')),
            Review = (float?)x.review,
            UserReview = (int?)x.userreview,
            Quantity = "0" //TODO: Quantity
        }
        );
    }

    public async Task<Product?> GetByIdAsync(Guid productId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var product = await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition("""
            select p.* , round(avg(r.review),1) as review, myr.review as userreview
            from product p 
                left join reviews r on p.id = r.productid
                left join reviews myr on p.id = myr.productid and myr.userid = @userId
            where p.id = @productId
            group by p.id,userreview
            """, new { productId ,userId}, cancellationToken: cancellationToken));
        if (product is null)
        {
            return null;
        }
        var fruitType = await connection.QueryAsync<string>(
            new CommandDefinition("""
            select type_name from fruit_type where id = @productId
            """, new { productId }, cancellationToken: cancellationToken));
        foreach (var type in fruitType)
        {
            product.FruitType.Add(type);
        }

        return product;
    }

    public async Task<Product?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var product = await connection.QuerySingleOrDefaultAsync<Product>(
        new CommandDefinition("""
            select p.* , round(avg(r.review),1) as review, myr.review as userreview
            from product p 
                left join reviews r on p.id = r.productid
                left join reviews myr on p.id = myr.productid and myr.userid = @userId
            where slug = @slug
            group by p.id,userreview
            """, new { slug ,userId}, cancellationToken: cancellationToken));
        if (product is null)
        {
            return null;
        }
        var fruitType = await connection.QueryAsync<string>(
            new CommandDefinition("""
            select type_name from fruit_type where id = @id
            """, new { id = product.Id }, cancellationToken: cancellationToken));
        foreach (var type in fruitType)
        {
            product.FruitType.Add(type);
        }

        return product;
    }

    public async Task<bool> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync(cancellationToken);
        var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update product set slug = @Slug, name = @Name where id = @Id
        """, product, transaction: transaction, cancellationToken: cancellationToken));

        if (result > 0)
        {
            // Delete existing fruit types for this product
            await connection.ExecuteAsync(new CommandDefinition("""
                delete from fruit_type where id = @Id
            """, new { product.Id }, transaction: transaction, cancellationToken: cancellationToken));

            // Insert new fruit types
            foreach (var fruitType in product.FruitType)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into fruit_type (id, type_name) values (@Id, @FruitType)
                """, new { Id = product.Id, FruitType = fruitType }, transaction: transaction, cancellationToken: cancellationToken));
            }
        }

        transaction.Commit();
        return result > 0;
    }
}