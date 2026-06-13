using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Mediator.Helper.Groups;
using Restaurant.Mediator.Helper.Settings;

namespace Restaurant.Web;

internal static class ConfigureServices
{
    public static IServiceCollection AddJwtConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSection = configuration.GetRequiredSection("JwtSettings");

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
        
        services.AddSingleton<IAuthorizationHandler, GroupHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, GroupPolicyProvider>();
        
        return services;
    }
}