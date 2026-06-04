using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Notifications.Models;

public sealed record NotificationDto(
    long Id,
    long UserId,
    NotificationType Type,
    long? OrderId,
    string? Message,
    bool IsRead,
    DateTime CreatedAt,
    DateTime UpdatedAt
);