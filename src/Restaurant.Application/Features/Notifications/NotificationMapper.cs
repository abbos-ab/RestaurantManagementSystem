using Restaurant.Application.Features.Notifications.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Notifications;

[Mapper]
public partial class NotificationMapper
{
    public partial NotificationDto Map(Notification notification);

    public partial List<NotificationDto> Map(List<Notification> notifications);
}