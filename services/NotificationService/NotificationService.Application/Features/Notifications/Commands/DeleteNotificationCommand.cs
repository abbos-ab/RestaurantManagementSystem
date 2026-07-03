using NotificationService.Application.Features.Notifications.Repositories;
using NotificationService.Application.Features.Notifications.Specifications;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace NotificationService.Application.Features.Notifications.Commands;

public sealed record DeleteNotificationCommand(
    long Id
) : ICommand;

internal sealed class DeleteNotificationCommandHandler : ICommandHandler<DeleteNotificationCommand>
{
    private readonly INotificationRepository _notificationRepository;

    public DeleteNotificationCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FirstOrDefaultAsync(
            new NotificationByIdSpec(request.Id),
            cancellationToken);

        if (notification is null)
            throw new BusinessLogicException(NotificationErrors.NotFound);

        await _notificationRepository.DeleteAsync(notification, cancellationToken);
    }
}