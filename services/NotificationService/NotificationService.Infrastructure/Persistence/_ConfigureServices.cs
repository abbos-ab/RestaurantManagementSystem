using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Persistence.Repositories;

namespace NotificationService.Infrastructure.Persistence;

internal static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddPersistenceServices(IConfiguration configuration, bool isDev)
        {
            services
                .AddRepositories();

            return services;
        }
    }
}