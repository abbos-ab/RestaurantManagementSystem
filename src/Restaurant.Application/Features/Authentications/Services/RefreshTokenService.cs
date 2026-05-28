using System.Buffers.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Restaurant.Application.Common.Extensions;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Authentications.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Settings;

namespace Restaurant.Application.Features.Authentications.Services;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    private const string ProviderName = "RefreshToken";

    private readonly TimeProvider _timeProvider;
    private readonly IDataProtector _dataProtector;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenService(
        TimeProvider timeProvider,
        IDataProtectionProvider dataProtectionProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> options)
    {
        _timeProvider = timeProvider;
        _dataProtector = dataProtectionProvider.CreateProtector(ProviderName);
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = options.Value;
    }

    public string CreateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var ms = new MemoryStream();

        using (var writer = ms.CreateWriter())
        {
            writer.Write(_timeProvider.GetUtcNow());
            writer.Write(user.Id.ToString());
        }

        var protectedBytes = _dataProtector.Protect(ms.ToArray());
        var token = Base64Url.EncodeToString(protectedBytes);

        return token;
    }

    public bool ValidateToken(User user, string refreshToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var unprotectedData = _dataProtector.Unprotect(Base64Url.DecodeFromChars(refreshToken));
            var ms = new MemoryStream(unprotectedData);
            using var reader = ms.CreateReader();

            var creationTime = reader.ReadDateTimeOffset();
            var expirationTime = creationTime + _jwtSettings.RefreshTokenLifeTime;

            if (expirationTime < _timeProvider.GetUtcNow())
                return false;

            var userIdAsString = reader.ReadString();

            if (!long.TryParse(userIdAsString, out var userId))
                return false;

            if (userId != user.Id)
                return false;

            return reader.PeekChar() == -1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> IsTokenUnique(string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var @params = new RefreshTokenByTokenParams
        {
            Token = token,
        };

        var refreshToken = await _refreshTokenRepository.FirstOrDefaultAsync(new RefreshTokenByTokenSpec(@params));

        return refreshToken == null;
    }

    public async Task RevokeToken(long userId, string refreshToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentNullException.ThrowIfNull(refreshToken);

        var @params = new RefreshTokenByTokenParams
        {
            Token = refreshToken,
            UserId = userId,
        };

        var token = await _refreshTokenRepository.FirstOrDefaultAsync(new RefreshTokenByTokenSpec(@params));
        if (token is null)
            return;

        await _refreshTokenRepository.DeleteAsync(token);

        await _refreshTokenRepository.SaveChangesAsync();
    }
}
