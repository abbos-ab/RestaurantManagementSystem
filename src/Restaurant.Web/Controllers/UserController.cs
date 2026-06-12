using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Users.Commands;
using Restaurant.Application.Features.Users.Models;
using Restaurant.Application.Features.Users.Queries;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Groups;

namespace Restaurant.Web.Controllers;

public class UsersController : BaseController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Create(
        CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPut]
    [GroupAuthorize(GroupNames.Administrators)]
    public async Task<ActionResult<UserDto>> Update(
        UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPut("{userId:long}")]
    [GroupAuthorize(GroupNames.Administrators)]
    public async Task<ActionResult<UserDto>> UpdateRole(
        long userId,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new UpdateUserRoleCommand(userId, role), cancellationToken));
    }

    [HttpDelete("{userId:long}")]
    [GroupAuthorize(GroupNames.Administrators)]
    public async Task<IActionResult> Delete(
        long userId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteUserCommand(userId), cancellationToken);
        return NoContent();
    }

    [HttpGet("{userId:long}")]
    public async Task<ActionResult<UserDto>> GetById(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken));
    }

    [HttpGet]
    [GroupAuthorize(GroupNames.Administrators)]
    public async Task<ActionResult<PaginatedResult<UserDto>>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetAllUsersQuery(paginationInfo), cancellationToken));
    }
}