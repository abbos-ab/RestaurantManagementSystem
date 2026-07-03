using Ardalis.Specification;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Notifications.Specifications;

public sealed class UserUnreadNotificationsSpec : Specification<Notification>
{
    public UserUnreadNotificationsSpec(long userId)
    {
        Query
            .Where(x => x.UserId == userId && !x.IsRead)
            .OrderByDescending(x => x.CreatedAt);
    }
}