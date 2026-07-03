using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Features.Notifications.Repositories;
using Restaurant.Mediator.Helper.Persistence;

namespace NotificationService.Infrastructure.Persistence.Repositories;

public static class ConfigureServices
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<NotificationDbContext>());
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        
        return services;
    }
}