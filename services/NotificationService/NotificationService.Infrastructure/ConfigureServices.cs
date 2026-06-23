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

        services.AddMassTransit(x =>
        {
            var rabbit = rabbitSettings.Get<RabbitMqSettings>()!;

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));

            x.AddConsumer<OrderPlacedConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<OrderUpdateConsumer>();
            x.AddConsumer<PaymentRequestedConsumer>();
            x.AddConsumer<PaymentStatusUpdatedConsumer>();
            x.AddConsumer<TableCalledWaiterConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbit.Host, rabbit.VirtualHost, h =>
                {
                    h.Username(rabbit.Username);
                    h.Password(rabbit.Password);
                });

                cfg.ConfigureEndpoints(context);

                cfg.UseJsonSerializer();
            });
        });

        return services;
    }
}