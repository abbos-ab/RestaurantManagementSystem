using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Common.Extensions;

namespace Restaurant.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddMappers();

        services.AddMediatR(x => x.RegisterServicesFromAssembly(ApplicationRef.Assembly));

        return services;
    }

}