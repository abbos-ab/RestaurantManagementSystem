using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;

namespace Restaurant.Infrastructure.Persistence.Seeders;

internal static class ConfigureServices
{
    internal static IServiceCollection AddSeeder(this IServiceCollection services)
    {
        services
            .AddScoped<IDatabaseSeeder, GroupDatabaseSeeder>();
        
        return services;
    }
}