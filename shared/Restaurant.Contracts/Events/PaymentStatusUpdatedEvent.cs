namespace Restaurant.Contracts.Events;

public sealed class PaymentStatusUpdatedEvent
{
    public required long PaymentId { get; init; }
    public required long OrderId { get; init; }
    public required long WaiterId { get; init; }
    public required string Status { get; init; }
    public required string Message { get; init; }
    public required DateTime UpdatedAt { get; init; }
}