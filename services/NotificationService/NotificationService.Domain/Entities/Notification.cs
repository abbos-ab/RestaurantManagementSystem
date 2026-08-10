namespace NotificationService.Domain.Entities;

public sealed class Notification
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public NotificationType Type { get; set; }

    public long? OrderId { get; set; }

    public string? Message { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum NotificationType
{
    OrderCreated = 0,
    OrderUpdated,
    OrderCancelled,

    OrderPreparing,
    OrderReady,

    OrderServed,

    TableCalledWaiter,
    TableChanged,

    PaymentRequested,
    PaymentCompleted,

    OrderItemStatusUpdated,
    OrderStatusUpdated,
}