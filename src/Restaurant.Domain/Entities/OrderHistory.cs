namespace Restaurant.Domain.Entities;

public sealed class OrderHistory : BaseEntity
{
    public long OrderId { get; set; }
    public Order Order { get; set; }

    public OrderHistoryAction Action { get; set; }

    public string? Description { get; set; }

    public long? UserId { get; set; }
    public User? User { get; set; }

    public long? OrderItemId { get; set; }
}

public enum OrderHistoryAction
{
    Created,
    ItemChanged,
    StatusChanged,
    Cancelled,
    Paid
}