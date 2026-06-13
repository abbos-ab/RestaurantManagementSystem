using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Persistence.Repositories;
using Restaurant.Infrastructure.Persistence.Seeders;

namespace Restaurant.Infrastructure.Persistence;

internal static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddPersistenceServices(IConfiguration configuration, bool isDev)
        {
            services
                .AddRepositories()
                .AddSeeder();

            return services;
        }
    }
}