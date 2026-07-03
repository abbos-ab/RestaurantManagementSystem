using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Features.Notifications.Commands;
using NotificationService.Application.Features.Notifications.Models;
using NotificationService.Application.Features.Notifications.Queries;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.Groups;

namespace NotificationService.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("user/{userId:long}")]
    public async Task<NotificationDto> GetNotificationsByUserId(
        GetNotificationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(query, cancellationToken);
    }

    [HttpPut]
    public async Task<ActionResult<NotificationDto>> Update(
        UpdateNotificationCommand command,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{notificationId:long}")]
    [GroupAuthorize(GroupNames.Administrators)]
    public async Task<IActionResult> Delete(
        long notificationId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteNotificationCommand(notificationId), cancellationToken);

        return NoContent();
    }

    [HttpGet("{notificationId:long}")]
    public async Task<ActionResult<NotificationDto>> GetById(
        long notificationId,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetNotificationByIdQuery(notificationId), cancellationToken));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<NotificationDto>>> GetAll(
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetAllNotificationsQuery(paginationInfo), cancellationToken));
    }

    [HttpGet("user/{userId:long}")]
    public async Task<ActionResult<PaginatedResult<NotificationDto>>> GetUserNotifications(
        long userId,
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetUserNotificationsQuery(userId, paginationInfo), cancellationToken));
    }

    [HttpGet("user/{userId:long}/unread")]
    public async Task<ActionResult<PaginatedResult<NotificationDto>>> GetUnread(
        long userId,
        [FromQuery] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetUnreadNotificationsQuery(userId, paginationInfo), cancellationToken));
    }

    [HttpPatch("{notificationId:long}/read")]
    public async Task<IActionResult> MarkAsRead(
        long notificationId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new MarkNotificationAsReadCommand(notificationId), cancellationToken);

        return NoContent();
    }

    [HttpPatch("user/{userId:long}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(
        long userId,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new MarkAllNotificationsAsReadCommand(userId), cancellationToken);

        return NoContent();
    }
}