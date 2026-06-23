using NotificationService.Application.Notifications.Models;
using NotificationService.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace NotificationService.Application.Notifications;

[Mapper]
public partial class NotificationMapper
{
    public partial NotificationDto Map(Notification notification);

    public partial List<NotificationDto> Map(List<Notification> notifications);
}