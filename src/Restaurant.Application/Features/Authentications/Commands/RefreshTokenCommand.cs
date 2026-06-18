using System.Security.Claims;
using Microsoft.Extensions.Options;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Authentications.Models;
using Restaurant.Application.Features.Authentications.Specifications;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Extensions;
using Restaurant.Mediator.Helper.Settings;

namespace Restaurant.Application.Features.Authentications.Commands;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand<AuthenticateResponse>;

// ReSharper disable once UnusedMember.Global
internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticateResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly TimeProvider _timeProvider;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService,
        TimeProvider timeProvider,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _timeProvider = timeProvider;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthenticateResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = _accessTokenService.GetClaimsFromExpiredToken(request.AccessToken);
        }
        catch (Exception e)
        {
            throw new UnauthorizedException(AuthErrors.Unauthorized, e);
        }

        var userId = principal.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
            throw new UnauthorizedException();

        var @params = new RefreshTokenByTokenParams
        {
            Token = request.RefreshToken,
            IncludeUser = true,
        };

        var refreshToken = await _refreshTokenRepository.FirstOrDefaultAsync(
            new RefreshTokenByTokenSpec(@params),
            cancellationToken
        );

        var refreshExpiresAt = _timeProvider
            .GetLocalDateTimeNowKindUtc()
            .Add(_jwtSettings.RefreshTokenLifeTime);

        var newRefreshToken = _refreshTokenService.CreateToken(user);
        var accessToken = await _accessTokenService.CreateToken(user);

        var isTokenUnique = await _refreshTokenService.IsTokenUnique(newRefreshToken);

        if (!isTokenUnique)
            newRefreshToken = _refreshTokenService.CreateToken(user);

        if (refreshToken is null)
        {
            await _refreshTokenRepository.AddAsync(
                new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = userId,
                    ExpiresAt = refreshExpiresAt,
                    CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
                },
                cancellationToken
            );

            await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

            return new TokenAuthenticationResponse(accessToken.accessToken, newRefreshToken, accessToken.expiresAt);
        }

        refreshToken.RevokedDate = _timeProvider.GetLocalDateTimeNowKindUtc();
        refreshToken.Token = newRefreshToken;
        refreshToken.ExpiresAt = refreshExpiresAt;

        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new TokenAuthenticationResponse(accessToken.accessToken, refreshToken.Token, accessToken.expiresAt);
    }
}
