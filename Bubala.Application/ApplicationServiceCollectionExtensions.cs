using Bubala.Application.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Bubala.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication (this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
        return services;
    }
}