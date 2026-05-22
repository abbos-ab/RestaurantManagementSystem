using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Restaurant.Mediator.Helper.Settings;

public class JwtSettings
{
    public required string Secret { get; init; }

    public required string Issuer { get; init; }

    public required TimeSpan AccessTokenLifeTime { get; init; }

    public required TimeSpan RefreshTokenLifeTime { get; init; }

    public SecurityKey GetSignInKey()
        => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
}
