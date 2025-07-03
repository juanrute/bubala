using Dapper;
namespace Bubala.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConectionAsync();
        await connection.ExecuteAsync("""
            create table if not exists product (
                id UUID primary key,
                slug text not null,
                name text not null
            ) 
        """);

        await connection.ExecuteAsync("""
            create unique index concurrently if not exists product_slug_idx
            on product
            using btree(slug);
        """);

        await connection.ExecuteAsync("""
            create table if not exists fruit_type (
                id UUID primary key,
                type_name text not null
            )
        """);

        await connection.ExecuteAsync("""
            create table if not exists reviews (
                userid uuid,
                productid uuid references product(id),
                review integer not null, 
                primary key (userid,productid)
            );            
        """);

    }
}