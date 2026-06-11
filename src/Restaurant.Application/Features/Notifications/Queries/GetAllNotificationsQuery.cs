using Ardalis.Specification;
using Restaurant.Application.Features.Notifications.Models;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Notifications.Queries;

public sealed record GetAllNotificationsQuery(
    PaginationInfo PaginationInfo
) : IQuery<PaginatedResult<NotificationDto>>;

internal sealed class GetAllNotificationsQueryHandler : IQueryHandler<GetAllNotificationsQuery, PaginatedResult<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationMapper _notificationMapper;

    public GetAllNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        NotificationMapper notificationMapper)
    {
        _notificationRepository = notificationRepository;
        _notificationMapper = notificationMapper;
    }

    public async Task<PaginatedResult<NotificationDto>> Handle(GetAllNotificationsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Notification>();

        spec.Query
            .Where(x => x.DeletedAt == null)
            .WithPagination(request.PaginationInfo);

        var notifications = await _notificationRepository.ListAsync(spec, cancellationToken);

        var totalCount = await _notificationRepository.CountAsync(spec, cancellationToken);

        var mappedNotifications = _notificationMapper.Map(notifications);

        return new PaginatedResult<NotificationDto>(mappedNotifications, totalCount);
    }
}