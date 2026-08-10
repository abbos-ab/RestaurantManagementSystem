using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;

namespace Restaurant.Infrastructure.Persistence.Seeders;

internal static class ConfigureServices
{
    internal static IServiceCollection AddSeeder(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseInitializer>();

        services
            .AddScoped<IDatabaseSeeder, GroupDatabaseSeeder>()
            .AddScoped<IDatabaseSeeder, CategoryDatabaseSeeder>()
            .AddScoped<IDatabaseSeeder, UserDatabaseSeeder>()
            .AddScoped<IDatabaseSeeder, DishDatabaseSeeder>()
            .AddScoped<IDatabaseSeeder, TableDatabaseSeeder>()
            .AddScoped<IDatabaseSeeder, UserDatabaseSeeder>()
            .AddScoped<IDatabaseSeeder, UserGroupRelationDatabaseSeeder>();
        
        return services;
    }
}