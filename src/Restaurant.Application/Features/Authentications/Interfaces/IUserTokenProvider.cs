using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Authentications.Interfaces;

public interface IUserTokenProvider
{
    string GenerateToken(User user, string? purpose);

    bool ValidateToken(User user, string token);
}
