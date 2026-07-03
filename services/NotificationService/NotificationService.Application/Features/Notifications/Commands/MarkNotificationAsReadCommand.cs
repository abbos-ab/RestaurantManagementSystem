using NotificationService.Application.Features.Notifications.Repositories;
using NotificationService.Application.Features.Notifications.Specifications;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace NotificationService.Application.Features.Notifications.Commands;

public sealed record MarkNotificationAsReadCommand(
    long Id
) : ICommand;

internal sealed class MarkNotificationAsReadCommandHandler
    : ICommandHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FirstOrDefaultAsync(
            new NotificationByIdSpec(request.Id),
            cancellationToken);

        if (notification is null)
            throw new BusinessLogicException(NotificationErrors.NotFound);

        if (notification.IsRead)
            throw new BusinessLogicException(NotificationErrors.AlreadyRead);

        notification.IsRead = true;

        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }
}