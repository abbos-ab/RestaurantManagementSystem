using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Infrastructure.Consumers;
using NotificationService.Infrastructure.Persistence;
using Restaurant.Mediator.Helper.Common.Settings;

namespace NotificationService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddNotificationInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDev)
    {
        var connectionString = configuration.GetConnectionString("Hangfire");

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddPersistenceServices(configuration, isDev);

        var rabbitSettings = configuration.GetRequiredSection(nameof(RabbitMqSettings));
        services.Configure<RabbitMqSettings>(rabbitSettings);

        services.AddMassTransitServices(configuration);

        return services;
    }
}