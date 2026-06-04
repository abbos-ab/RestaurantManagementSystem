using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Notifications.Specifications;

public sealed class UserNotificationsSpec : Specification<Notification>
{
    public UserNotificationsSpec(long userId)
    {
        Query
            .Where(x =>
                x.UserId == userId &&
                x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt);
    }
}