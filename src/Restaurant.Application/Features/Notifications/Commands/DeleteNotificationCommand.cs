using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Application.Features.Notifications.Specifications;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Notifications.Commands;

public sealed record DeleteNotificationCommand(
    long Id
) : ICommand;

internal sealed class DeleteNotificationCommandHandler : ICommandHandler<DeleteNotificationCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly TimeProvider _timeProvider;

    public DeleteNotificationCommandHandler(
        INotificationRepository notificationRepository,
        TimeProvider timeProvider)
    {
        _notificationRepository = notificationRepository;
        _timeProvider = timeProvider;
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FirstOrDefaultAsync(
            new NotificationByIdSpec(request.Id),
            cancellationToken);

        if (notification is null)
            throw new BusinessLogicException(NotificationErrors.NotFound);

        notification.DeletedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        notification.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}