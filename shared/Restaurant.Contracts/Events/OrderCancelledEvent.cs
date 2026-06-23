namespace Restaurant.Contracts.Events;

public sealed class OrderCancelledEvent
{
    public required long OrderId { get; init; }
    public required long? UserId { get; init; }
    public required string Reason { get; init; }
    public required DateTime CancelledAt { get; init; }
}