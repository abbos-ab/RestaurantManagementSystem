using Restaurant.Mediator.Helper.Common.Models;

namespace NotificationService.Application.Features.Notifications;

public static class NotificationErrors
{
    public static readonly Error NotFound = new(
        "Notification.NotFound",
        "Notification not found."
    );
    
    public static readonly Error AlreadyRead = new(
        "Notification.AlreadyRead",
        "Notification has already been read."
    );
}