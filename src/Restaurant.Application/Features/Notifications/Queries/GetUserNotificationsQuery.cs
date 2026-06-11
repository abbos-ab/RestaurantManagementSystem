using Ardalis.Specification;
using Restaurant.Application.Features.Notifications.Models;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Notifications.Queries;

public sealed record GetUserNotificationsQuery(
    long UserId,
    PaginationInfo PaginationInfo
) : IQuery<PaginatedResult<NotificationDto>>;

internal sealed class
    GetUserNotificationsQueryHandler : IQueryHandler<GetUserNotificationsQuery, PaginatedResult<NotificationDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly NotificationMapper _notificationMapper;

    public GetUserNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        NotificationMapper notificationMapper)
    {
        _notificationRepository = notificationRepository;
        _notificationMapper = notificationMapper;
    }

    public async Task<PaginatedResult<NotificationDto>> Handle(GetUserNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Notification>();

        spec.Query
            .Where(x =>
                x.UserId == request.UserId &&
                x.DeletedAt == null)
            .WithPagination(request.PaginationInfo);

        var notifications = await _notificationRepository.ListAsync(spec, cancellationToken);

        var countSpec = new ReadOnlySpecification<Notification>();

        countSpec.Query
            .Where(x =>
                x.UserId == request.UserId &&
                x.DeletedAt == null);

        var totalCount = await _notificationRepository.CountAsync(countSpec, cancellationToken);

        var mapped = _notificationMapper.Map(notifications);

        return new PaginatedResult<NotificationDto>(mapped, totalCount);
    }
}