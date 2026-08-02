using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Restaurant.Application.Features.Authentications;
using Restaurant.Application.Features.Authentications.Commands;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Application.Features.Users.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Utils;

namespace Restaurant.UnitTests.Commands.Authentications;

public class ChangePasswordCommandTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserTokenProvider _userTokenProvider;

    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandTests()
    {
        var fixture = new Fixture().WithAutoNSubstitutions();

        _userRepository = fixture.Freeze<IUserRepository>();
        _passwordHasher = fixture.Freeze<IPasswordHasher<User>>();
        _userTokenProvider = fixture.Freeze<IUserTokenProvider>();

        _handler = fixture.Create<ChangePasswordCommandHandler>();
    }

    [Fact]
    public async Task UserIsNull_ShouldThrow()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new ChangePasswordCommand(
            "+992987654321",
            fixture.Create<string>(),
            fixture.Create<string>()
        );

        _userRepository
            .FirstOrDefaultAsync(Arg.Any<UserByPhoneSpec>())
            .ReturnsNull();

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<UnauthorizedException>();
        exception.Which.Error.Should().Be(AuthErrors.Unauthorized);
    }

    [Fact]
    public async Task UserTokenIsNotValidate_ShouldThrow()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new ChangePasswordCommand(
            "+992987654321",
            fixture.Create<string>(),
            fixture.Create<string>()
        );

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

        _userTokenProvider
            .ValidateToken(user, command.Token)
            .Returns(false);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<UnauthorizedException>();
        exception.Which.Error.Should().Be(AuthErrors.Unauthorized);
    }

    [Fact]
    public async Task PasswordShouldBeNew_ShouldThrow()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new ChangePasswordCommand(
            "+992987654321",
            fixture.Create<string>(),
            fixture.Create<string>()
        );

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

        var newPasswordHash = user.Password;

        _userRepository
            .FirstOrDefaultAsync(Arg.Any<UserByPhoneSpec>())
            .Returns(user);

        _userTokenProvider
            .ValidateToken(user, command.Token)
            .Returns(true);

        _passwordHasher
            .HashPassword(user, command.NewPassword)
            .Returns(newPasswordHash);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        var exception = await action.Should().ThrowAsync<BusinessLogicException>();
        exception.Which.Error.Should().Be(AuthErrors.PasswordShouldBeNew);
    }

    [Fact]
    public async Task ChangePassword_ShouldBeSuccess()
    {
        // Arrange
        var fixture = new Fixture();
        var command = new ChangePasswordCommand(
            "+992987654321",
            fixture.Create<string>(),
            fixture.Create<string>()
        );

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

        var newPasswordHash = fixture.Create<string>();

        _userRepository
            .FirstOrDefaultAsync(Arg.Any<UserByPhoneSpec>())
            .Returns(user);

        _userTokenProvider
            .ValidateToken(user, command.Token)
            .Returns(true);

        _passwordHasher
            .HashPassword(user, command.NewPassword)
            .Returns(newPasswordHash);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.Password.Should().Be(newPasswordHash);
        user.IsActive.Should().Be(true);

        _ = _userRepository
            .Received()
            .FirstOrDefaultAsync(Arg.Is<UserByPhoneSpec>(x => x.Phone == PhoneNumber.Create(command.PhoneNumber)));

        _ = _userTokenProvider
            .Received()
            .ValidateToken(user, command.Token);

        _passwordHasher
            .Received()
            .HashPassword(user, command.NewPassword);
    }
}