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

    public async Task<bool> CreateAsync(Product product)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into product (id, slug, name) values (@Id, @Slug, @Name)
        """, product));
        if (result > 0)
        {
            foreach (var fruitType in product.FruitType)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into fruit_type (id, type_name) Values (@Id, @FruitType);
                """, new { Id = product.Id, FruitType = product.FruitType }));    

            }
        }
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from product where id=@id
        """,new { id }));
        if (result > 0)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
            delete from fruit_type where id = @id
            """, new { id }));
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from product where id=@id
        """,new { id }));
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        var result = await connection.QueryAsync(new CommandDefinition("""
            select p.*, string_agg(ft.type_name,',') as types
            from product as p left join fruit_type as ft on p.id = ft.id
            group by p.id
        """));

        return result.Select(x => new Product
        {
            Id = x.id,
            Name = x.name,
            FruitType = Enumerable.ToList(x.types.Split(',')),
            Quantity = "0" //TODO: Quantity
        }
        );
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        var product = await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition("""
            select * from product where id = @id
            """, new { id }));
        if (product is null)
        {
            return null;
        }
        var fruitType = await connection.QueryAsync<string>(
            new CommandDefinition("""
            select * from fruit_type where id = @id
            """, new { id }));
        foreach (var type in fruitType)
        {
            product.FruitType.Add(type);
        }

        return product;
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        var product = await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition("""
            select * from product where slug = @slug
            """, new { slug }));
        if (product is null)
        {
            return null;
        }
        var fruitType = await connection.QueryAsync<string>(
            new CommandDefinition("""
            select * from fruit_type where id = @id
            """, new { id = product.Id }));
        foreach (var type in fruitType)
        {
            product.FruitType.Add(type);
        }

        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        var transaction = connection.BeginTransaction();
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update product set slug = @Slug, name = @Name where id = @Id
        """, product, transaction: transaction));

        if (result > 0)
        {
            // Delete existing fruit types for this product
            await connection.ExecuteAsync(new CommandDefinition("""
                delete from fruit_type where id = @Id
            """, new { product.Id }, transaction: transaction));

            // Insert new fruit types
            foreach (var fruitType in product.FruitType)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into fruit_type (id, type_name) values (@Id, @FruitType)
                """, new { Id = product.Id, FruitType = fruitType }, transaction: transaction));
            }
        }

        transaction.Commit();
        return result > 0;
    }
}