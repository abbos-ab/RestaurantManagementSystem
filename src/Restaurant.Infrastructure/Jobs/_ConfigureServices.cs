using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Restaurant.Infrastructure.Jobs;

internal static class ConfigureServices
{
    internal static IServiceCollection AddJobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RemoveExpiredRefreshTokens>();

        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(
                    configuration.GetConnectionString("DefaultConnection")!
                )
            )
        );

        return services;
    }
}