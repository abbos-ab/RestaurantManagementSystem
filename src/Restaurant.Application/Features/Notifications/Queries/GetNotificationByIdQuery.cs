using Restaurant.Application.Features.Notifications.Models;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Application.Features.Notifications.Specifications;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Notifications.Queries;

public sealed record GetNotificationByIdQuery(
    long Id
) : IQuery<NotificationDto>;

internal sealed class GetNotificationByIdQueryHandler : IQueryHandler<GetNotificationByIdQuery, NotificationDto>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationMapper _notificationMapper;

    public GetNotificationByIdQueryHandler(
        INotificationRepository notificationRepository,
        NotificationMapper notificationMapper)
    {
        _notificationRepository = notificationRepository;
        _notificationMapper = notificationMapper;
    }

    public async Task<NotificationDto> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.FirstOrDefaultAsync(new NotificationByIdSpec(request.Id), cancellationToken);

        if (notification is null) throw new BusinessLogicException(NotificationErrors.NotFound);

        return _notificationMapper.Map(notification);
    }
}