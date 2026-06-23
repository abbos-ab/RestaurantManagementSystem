using NotificationService.Application.Notifications.Repositories;
using NotificationService.Application.Notifications.Specifications;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace NotificationService.Application.Notifications.Commands;

public sealed record MarkAllNotificationsAsReadCommand(
    long UserId
) : ICommand;

internal sealed class MarkAllNotificationsAsReadCommandHandler : ICommandHandler<MarkAllNotificationsAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var notifications =
            await _notificationRepository.ListAsync(
                new UserUnreadNotificationsSpec(request.UserId),
                cancellationToken);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _notificationRepository.UpdateRangeAsync(notifications, cancellationToken);
    }
}