using Ardalis.Specification;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Notifications.Specifications;

public sealed class UserNotificationsSpec : Specification<Notification>
{
    public UserNotificationsSpec(long userId)
    {
        Query
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt);
    }
}