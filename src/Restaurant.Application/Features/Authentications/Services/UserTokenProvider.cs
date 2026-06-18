using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Restaurant.Application.Common.Extensions;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Settings;

namespace Restaurant.Application.Features.Authentications.Services;

internal sealed class UserTokenProvider : IUserTokenProvider
{
    private readonly TimeProvider _timeProvider;
    private readonly IDataProtector _dataProtector;
    private readonly UserTokenProviderSettings _providerSettings;

    public UserTokenProvider(
        TimeProvider timeProvider,
        IDataProtectionProvider dataProtectionProvider,
        IOptions<UserTokenProviderSettings> options)
    {
        _timeProvider = timeProvider;
        _providerSettings = options.Value;
        _dataProtector = dataProtectionProvider.CreateProtector(_providerSettings.Name);
    }

    public string GenerateToken(User user, string? purpose)
    {
        ArgumentNullException.ThrowIfNull(user);

        var ms = new MemoryStream();
        using (var writer = ms.CreateWriter())
        {
            writer.Write(_timeProvider.GetUtcNow());
            writer.Write(user.Id.ToString());
            writer.Write(purpose ?? "");
        }

        var protectedBytes = _dataProtector.Protect(ms.ToArray());
        return Convert.ToBase64String(protectedBytes);
    }

    public bool ValidateToken(User user, string token)
    {
        ArgumentNullException.ThrowIfNull(user);

        try
        {
            var unsupportedData = _dataProtector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unsupportedData);
            using var reader = ms.CreateReader();

            var creationTime = reader.ReadDateTimeOffset();
            var expirationTime = creationTime + _providerSettings.TokenLifeTime;

            if (expirationTime < _timeProvider.GetUtcNow())
                return false;

            var userIdStr = reader.ReadString();
            if (!long.TryParse(userIdStr, out var userId))
                return false;

            if (userId != user.Id)
                return false;

            return reader.PeekChar() == -1;
        }
        catch
        {
            return false;
        }
    }
}
