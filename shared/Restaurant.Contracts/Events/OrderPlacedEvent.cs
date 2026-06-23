namespace Restaurant.Contracts.Events;

public sealed class OrderPlacedEvent
{
    public required long OrderId { get; init; }
    public required long? UserId { get; init; }
    public required string CustomerName { get; init; }
    public required decimal TotalAmount { get; init; }
}