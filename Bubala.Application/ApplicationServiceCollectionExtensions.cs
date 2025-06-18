using Bubala.Application.Repositories;
using Bubala.Application.Database;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.Internal;

namespace Bubala.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
        return services;
    }

    public static IServiceCollection AddDataBase(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}