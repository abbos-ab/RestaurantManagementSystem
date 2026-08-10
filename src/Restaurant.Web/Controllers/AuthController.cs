using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Authentications.Commands;
using Restaurant.Application.Features.Authentications.Models;
using Restaurant.Mediator.Helper.Extensions;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<AuthenticateResponse> Authenticate(
        [FromBody] AuthenticateCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPost("refresh")]
    public async Task<AuthenticateResponse> RefreshToken(
        [FromBody] RefreshTokenCommand command)
    {
        return await _mediator.Send(command);
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<ActionResult> Revoke(
        [FromBody] RevokeTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!User.TryGetUserId(out var userId))
            return BadRequest();

        await _mediator.Send(
            new RevokeTokenCommand(userId, request.RefreshToken),
            cancellationToken);

        return NoContent();
    }

    [Authorize]
    [HttpPut("changePassword")]
    public async Task<NoContentResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}