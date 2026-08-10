using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Restaurant.Application.Common.Behaviors;
using Restaurant.Application.Common.Context;
using Restaurant.Application.Common.Extensions;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Authentications.Services;
using Restaurant.Mediator.Helper;
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

        services
            .AddMemoryCache()
            .AddMappers();

        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(ApplicationRef.Assembly); });

        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));

        services.AddValidatorsFromAssembly(ApplicationRef.Assembly);

        services.TryAddSingleton<ICurrentUserAccessor, CurrentUserAccessor>();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IRequestContext, RequestContext>();
        
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }
}