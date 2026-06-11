using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Application.Services;
using Restaurant.Infrastructure.Settings;
using Minio;

namespace Restaurant.Infrastructure.Services;

internal static class ConfigureServices
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddServices(IConfiguration configuration)
        {
            services.AddScoped<IImageCompressionService, ImageCompressionService>();
            
            return services.AddMinioServices(configuration);
        }

        private IServiceCollection AddMinioServices(IConfiguration configuration)
        {
            var minioSettings = configuration.GetRequiredSection(nameof(MinioSettings));
            services.Configure<MinioSettings>(minioSettings);

            services.AddMinio(x =>
                {
                    var settings = minioSettings.Get<MinioSettings>()!;

                    x.SetAppInfo(typeof(InfrastructureRef).FullName, "1");
                    x.WithEndpoint(settings.Endpoint);
                    x.WithCredentials(settings.AccessKey, settings.SecretKey);
                    x.WithRegion(settings.Region);
                    x.WithSSL(settings.Secure);
                }
            );

            services.AddScoped<IMinioClientService, MinioClientService>();

            return services;
        }
    }
}