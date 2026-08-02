using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Restaurant.Application.Features.Authentications;
using Restaurant.Application.Features.Authentications.Commands;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Authentications.Models;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Users.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;
using Restaurant.Mediator.Helper.Settings;
using Restaurant.Mediator.Helper.Utils;

namespace Restaurant.UnitTests.Commands.Authentications;

//TODO intersepter

public class AuthenticateCommandTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FakeTimeProvider _fakeTimeProvider = new();

    private readonly AuthenticateCommandHandler _handler;

    public AuthenticateCommandTests()
    {
        var fixture = new Fixture().WithAutoNSubstitutions();
        fixture.Register<TimeProvider>(() => _fakeTimeProvider);

        fixture
            .Freeze<IOptions<JwtSettings>>()
            .Value
            .Returns(
                new JwtSettings
                {
                    Secret = string.Join(string.Empty, Enumerable.Range(1, 64).Select(_ => '0')),
                    Issuer = fixture.Create<string>(),
                    AccessTokenLifeTime = TimeSpan.FromMinutes(10),
                    RefreshTokenLifeTime = TimeSpan.FromMinutes(1),
                }
            );

        _accessTokenService = fixture.Freeze<IAccessTokenService>();
        _userRepository = fixture.Freeze<IUserRepository>();
        _passwordHasher = fixture.Freeze<IPasswordHasher<User>>();
        _refreshTokenRepository = fixture.Freeze<IRefreshTokenRepository>();
        _refreshTokenService = fixture.Freeze<IRefreshTokenService>();
        _unitOfWork = fixture.Freeze<IUnitOfWork>();

        _handler = fixture.Create<AuthenticateCommandHandler>();
    }

    [Fact]
    public async Task UserNotFound_ShouldThrow()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new AuthenticateCommand("+992987654321", fixture.Create<string>());

        _userRepository
            .FirstOrDefaultAsync(Arg.Any<UserByPhoneSpec>())
            .ReturnsNull();

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<UnauthorizedException>();
        exception.Which.Error.Should().Be(AuthErrors.InvalidCredentials);
    }

    [Fact]
    public async Task UserExists_ButNotActive_ShouldThrow()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new AuthenticateCommand("+992987654321", fixture.Create<string>());

        var user = new User
        {
            PhoneNumber = fixture.Create<PhoneNumber>(),
            Password = command.Password,
            IsActive = false,
        };


        _userRepository
            .FirstOrDefaultAsync(Arg.Any<UserByPhoneSpec>())
            .Returns(user);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<AccessDeniedException>();
        exception.Which.Error.Should().Be(AuthErrors.UserIsDisabled);
    }

    [Fact]
    public async Task PasswordVerificationResultFailed_ShouldThrow()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new AuthenticateCommand("+992987654321", fixture.Create<string>());

        var user = new User
        {
            PhoneNumber = fixture.Create<PhoneNumber>(),
            Password = command.Password,
            IsActive = false,
        };

        _passwordHasher
            .VerifyHashedPassword(user, user.Password, command.Password)
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<UnauthorizedException>();
        exception.Which.Error.Should().Be(AuthErrors.InvalidCredentials);
    }

    [Fact]
    public async Task RefreshTokenNotUnique_ShouldReturnLoginResponse()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new AuthenticateCommand("+992987654321", fixture.Create<string>());

        var user = new User
        {
            Id = 1,
            FirstName = "Иванов",
            LastName = "Иван",
            Email = "app@gmail.com",
            PhoneNumber = fixture.Create<PhoneNumber>(),
            Password = "password",
            IsActive = true,
        };

        _userRepository
            .FirstOrDefaultAsync(Arg.Any<UserByPhoneSpec>())
            .Returns(user);

        var dateTimeOffsetNow = DateTimeOffset.Now;
        var now = DateTime.SpecifyKind(dateTimeOffsetNow.UtcDateTime, DateTimeKind.Utc);
        _fakeTimeProvider.SetUtcNow(dateTimeOffsetNow);

        _passwordHasher
            .VerifyHashedPassword(user, user.Password, command.Password)
            .Returns(PasswordVerificationResult.Success);

        var accessToken = fixture.Create<string>();
        _accessTokenService
            .CreateToken(user)
            .Returns((now.AddMinutes(1), accessToken));

        var refreshToken = fixture.Create<string>();
        var newRefreshToken = fixture.Create<string>();
        _refreshTokenService
            .CreateToken(user)
            .Returns(refreshToken, newRefreshToken);

        _refreshTokenService
            .IsTokenUnique(refreshToken)
            .Returns(false);

        RefreshToken refreshTokenEntity = null!;
        _refreshTokenRepository
            .When(x => x.AddAsync(Arg.Any<RefreshToken>()))
            .Do(x => refreshTokenEntity = x.Arg<RefreshToken>());

        // Act
        AuthenticateResponse response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _ = _passwordHasher
            .Received()
            .VerifyHashedPassword(user, user.Password, command.Password);

        _ = _accessTokenService
            .Received()
            .CreateToken(user);

        _ = _refreshTokenService
            .Received()
            .CreateToken(user);

        _ = _refreshTokenRepository
            .Received()
            .AddAsync(refreshTokenEntity);

        _ = _unitOfWork
            .Received()
            .SaveChangesAsync();

        using (new AssertionScope())
        {
            refreshTokenEntity.CreatedAt.Should().Be(now);
            refreshTokenEntity.Token.Should().Be(newRefreshToken);
            refreshTokenEntity.UserId.Should().Be(user.Id);
        }
    }
}