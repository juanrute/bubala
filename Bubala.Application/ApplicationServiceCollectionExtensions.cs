using Bubala.Application.Repositories;
using Bubala.Application.Database;
using Microsoft.Extensions.DependencyInjection;
using Bubala.Application.Services;
using FluentValidation;

namespace Bubala.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IReviewRepository, ReviewRepository>();
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
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