using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Notifications.Specifications;

public sealed class NotificationByIdSpec : Specification<Notification>
{
    public NotificationByIdSpec(long id)
    {
        Query.Where(x =>
            x.Id == id &&
            x.DeletedAt == null);
    }
}