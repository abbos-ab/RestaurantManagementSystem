namespace Restaurant.Contracts.Events;

public sealed class OrderUpdatedEvent
{
    public required long OrderId { get; init; }
    public required long? UserId { get; init; }
    public required decimal TotalAmount { get; init; }
    public required string UpdateDescription { get; init; }
    public required DateTime UpdatedAt { get; init; }
}