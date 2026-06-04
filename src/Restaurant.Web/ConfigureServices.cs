using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Mediator.Helper.Settings;

namespace Restaurant.Web;

internal static class ConfigureServices
{
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetRequiredSection("JwtSettings");

        // direct DI uchun
        var jwtSettings = jwtSection.Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JwtSettings is missing");

        services.AddSingleton(jwtSettings);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtSettings.GetSignInKey(),

                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = false
                };
            });

        services.AddAuthorization();

        return services;
    }
}