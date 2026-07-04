using AutoFixture;
using NSubstitute;
using Restaurant.Application.Features.Authentications.Commands;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.UnitTests.Utils;

namespace Restaurant.UnitTests.Commands.Authentications;

public class RevokeTokenCommandTests
{
    private readonly IRefreshTokenService _refreshTokenService;

    private readonly RevokeTokenCommandHandler _handler;

    public RevokeTokenCommandTests()
    {
        var fixture = new Fixture().WithAutoNSubstitutions();

        _refreshTokenService = fixture.Freeze<IRefreshTokenService>();

        _handler = fixture.Create<RevokeTokenCommandHandler>();
    }

    [Fact]
    public async Task RevokeToken_ShouldBeSuccess()
    {
        // Arrange
        var fixture = new Fixture();
        var command = fixture.Create<RevokeTokenCommand>();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _ = _refreshTokenService
            .Received()
            .RevokeToken(command.UserId, command.RefreshToken);
    }
}