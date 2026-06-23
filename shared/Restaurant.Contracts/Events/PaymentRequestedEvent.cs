namespace Restaurant.Contracts.Events;

public sealed class PaymentRequestedEvent
{
    public required long OrderId { get; init; }
    public required long WaiterId { get; init; }
    public required decimal Amount { get; init; }
    public required string Message { get; init; }
    public required DateTime CreatedAt { get; init; }
}