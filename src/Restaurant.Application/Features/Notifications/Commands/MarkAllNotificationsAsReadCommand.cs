using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Application.Features.Notifications.Specifications;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.Notifications.Commands;

public sealed record MarkAllNotificationsAsReadCommand(
    long UserId
) : ICommand;

internal sealed class MarkAllNotificationsAsReadCommandHandler : ICommandHandler<MarkAllNotificationsAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly TimeProvider _timeProvider;

    public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository, TimeProvider timeProvider)
    {
        _notificationRepository = notificationRepository;
        _timeProvider = timeProvider;
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
            notification.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();
        }

        await _notificationRepository.UpdateRangeAsync(notifications, cancellationToken);
    }
}