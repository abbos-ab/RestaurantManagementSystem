namespace Restaurant.Contracts.Events;

public sealed class TableCalledWaiterEvent
{
    public required long TableId { get; init; }
    public required long WaiterId { get; init; }
    public required DateTime CalledAt { get; init; }
}