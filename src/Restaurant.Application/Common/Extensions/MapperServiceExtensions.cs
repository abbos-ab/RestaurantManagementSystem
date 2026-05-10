using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Restaurant.Application.Common.Extensions;

public static class MapperServiceExtensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        var assembly = ApplicationRef.Assembly;

        var mapperTypes = assembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("Mapper")
                        && t is { IsAbstract: false, IsInterface: false }
            );

        foreach (var mapperType in mapperTypes)
            services.AddSingleton(mapperType);

        return services;
    }
}