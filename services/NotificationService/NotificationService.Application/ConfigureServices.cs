using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Common.Extensions;
using Restaurant.Mediator.Helper.Behaviors;

namespace NotificationService.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        
        services.AddMappers();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(ApplicationRef.Assembly);
        });
        
        services.AddValidatorsFromAssembly(ApplicationRef.Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        
        return services;
    }
}