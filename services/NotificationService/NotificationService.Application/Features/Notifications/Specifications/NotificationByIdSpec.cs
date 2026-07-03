using Ardalis.Specification;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Features.Notifications.Specifications;

public sealed class NotificationByIdSpec : Specification<Notification>
{
    public NotificationByIdSpec(long id)
    {
        Query.Where(x => x.Id == id);
    }
}