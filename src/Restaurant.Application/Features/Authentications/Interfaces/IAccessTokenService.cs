using Restaurant.Domain.Entities;
using System.Security.Claims;

namespace Restaurant.Application.Features.Authentications.Interfaces;

public interface IAccessTokenService
{
    Task<(DateTime expiresAt, string accessToken)> CreateToken(User user);

    ClaimsPrincipal GetClaimsFromExpiredToken(string accessToken);
}
