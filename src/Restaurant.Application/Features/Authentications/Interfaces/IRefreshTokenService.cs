using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Authentications.Interfaces;

public interface IRefreshTokenService
{
    string CreateToken(User user);

    bool ValidateToken(User user, string refreshToken);

    Task RevokeToken(long userId, string refreshToken);

    Task<bool> IsTokenUnique(string token);
}