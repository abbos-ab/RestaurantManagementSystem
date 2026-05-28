using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Authentications.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Users.Specifications;
using Restaurant.Application.Features.Users.Validators;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;
using Restaurant.Mediator.Helper.Settings;

namespace Restaurant.Application.Features.Authentications.Commands;

public sealed record AuthenticateCommand(
    string PhoneNumber,
    string Password
) : ICommand<AuthenticateResponse>;

// ReSharper disable once UnusedType.Global
public sealed class AuthenticateCommandValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticateCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).SetValidator(new PhoneValidator());
        RuleFor(x => x.Password).NotEmpty();
    }
}

internal sealed class AuthenticateCommandHandler : ICommandHandler<AuthenticateCommand, AuthenticateResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;
    private readonly JwtSettings _jwtSettings;

    public AuthenticateCommandHandler(
        IUserRepository userRepository,
        TimeProvider timeProvider,
        IPasswordHasher<User> passwordHasher,
        IAccessTokenService accessTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        JwtSettings jwtSettings,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _passwordHasher = passwordHasher;
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticateResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(request.PhoneNumber);

        var user = await _userRepository.FirstOrDefaultAsync(
            new UserByPhoneSpec(phone),
            cancellationToken
        );

        if (user is null)
            throw new UnauthorizedException(AuthErrors.InvalidCredentials);

        var now = _timeProvider.GetLocalDateTimeNowKindUtc();

        if (!user.IsActive)
            throw new AccessDeniedException(AuthErrors.UserIsDisabled);

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);
        if (verificationResult is PasswordVerificationResult.Failed)
            throw new UnauthorizedException(AuthErrors.InvalidCredentials);

        var accessToken = await _accessTokenService.CreateToken(user);
        var refreshToken = _refreshTokenService.CreateToken(user);

        var isRefreshTokenUnique = await _refreshTokenService.IsTokenUnique(refreshToken);

        if (!isRefreshTokenUnique)
            refreshToken = _refreshTokenService.CreateToken(user);

        var refreshExpiresAt = _timeProvider.GetLocalDateTimeNowKindUtc().Add(_jwtSettings.RefreshTokenLifeTime);

        await _refreshTokenRepository.AddAsync(
            new RefreshToken
            {
                CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = refreshExpiresAt,
            },
            cancellationToken
        );

        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        return new TokenAuthenticationResponse(accessToken.accessToken, refreshToken, accessToken.expiresAt);
    }
}