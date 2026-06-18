using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.Authentications.Commands;


public sealed record RevokeTokenCommand(long UserId, string RefreshToken) : ICommand;

internal sealed class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RevokeTokenCommandHandler(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        await _refreshTokenService.RevokeToken(request.UserId, request.RefreshToken);
    }
}
