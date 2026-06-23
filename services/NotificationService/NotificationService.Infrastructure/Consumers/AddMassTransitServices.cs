using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Mediator.Helper.Common.Settings;

namespace NotificationService.Infrastructure.Consumers;

internal static class ConfigureServices
{
    internal static IServiceCollection AddMassTransitServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rabbitSettings = configuration.GetRequiredSection(nameof(RabbitMqSettings));
        services.Configure<RabbitMqSettings>(rabbitSettings);

        services.AddMassTransit(
            x =>
            {
                var rabbit = rabbitSettings.Get<RabbitMqSettings>()!;

                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));

                x.AddConsumer<OrderPlacedConsumer>();
                x.AddConsumer<OrderCancelledConsumer>();
                x.AddConsumer<OrderUpdateConsumer>();
                x.AddConsumer<PaymentRequestedConsumer>();
                x.AddConsumer<PaymentStatusUpdatedConsumer>();
                x.AddConsumer<TableCalledWaiterConsumer>();

                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        cfg.Host(
                            rabbit.Host,
                            rabbit.VirtualHost,
                            h =>
                            {
                                h.Username(rabbit.Username);
                                h.Password(rabbit.Password);
                            }
                        );

                        cfg.ConfigureEndpoints(context);
                        cfg.UseNewtonsoftJsonSerializer();
                        cfg.UseNewtonsoftJsonDeserializer();
                    }
                );
            }
        );

        return services;
    }
}