using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Common.Interfaces;
using Restaurant.Infrastructure.Jobs;
using Restaurant.Infrastructure.Notifications.Telegram;
using Restaurant.Infrastructure.Persistence;
using Restaurant.Infrastructure.Producers;
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
            .AddPersistenceServices(configuration, isDev)
            .AddJobServices(configuration)
            .AddServices(configuration)
            .AddRabbitMqServices(configuration);

        services.Configure<TelegramOptions>(configuration.GetSection(TelegramOptions.SectionName));
        services.AddSingleton<ITelegramBotService, TelegramBotService>();
        services.AddSingleton<IExceptionNotifier, TelegramExceptionNotifier>();

        return services;
    }
}