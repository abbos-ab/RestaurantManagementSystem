using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Common.Extensions;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Authentications.Services;
using Restaurant.Application.Features.Dishes.Queries;
using Restaurant.Mediator.Helper.Behaviors;

namespace Restaurant.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        
        services
            .AddScoped<IAccessTokenService, AccessTokenService>()
            .AddScoped<IUserTokenProvider, UserTokenProvider>()
            .AddScoped<IRefreshTokenService, RefreshTokenService>();
        
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