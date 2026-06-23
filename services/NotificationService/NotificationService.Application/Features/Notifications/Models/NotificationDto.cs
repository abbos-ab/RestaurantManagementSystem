using NotificationService.Domain.Entities;

namespace NotificationService.Application.Notifications.Models;

public sealed record NotificationDto(
    long Id,
    long UserId,
    NotificationType Type,
    long? OrderId,
    string? Message,
    bool IsRead,
    DateTime CreatedAt
);