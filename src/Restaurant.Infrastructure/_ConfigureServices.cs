using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Infrastructure.Jobs;
using Restaurant.Infrastructure.Services;

namespace Restaurant.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDev)
    {
        services
            .AddJobServices(configuration)
            .AddServices(configuration);
        
        return services;
    }
}