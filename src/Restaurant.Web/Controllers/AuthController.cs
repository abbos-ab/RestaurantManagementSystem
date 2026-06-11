using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Authentications.Commands;
using Restaurant.Application.Features.Authentications.Models;

namespace Restaurant.Web.Controllers;

public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<AuthenticateResponse> Authenticate([FromBody] AuthenticateCommand command)
    {
        return await _mediator.Send(command);
    }
}