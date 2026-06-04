using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Notifications.Specifications;

public sealed class UserUnreadNotificationsSpec : Specification<Notification>
{
    public UserUnreadNotificationsSpec(long userId)
    {
        Query
            .Where(x =>
                x.UserId == userId &&
                !x.IsRead &&
                x.DeletedAt == null)
            .OrderByDescending(x => x.CreatedAt);
    }
}