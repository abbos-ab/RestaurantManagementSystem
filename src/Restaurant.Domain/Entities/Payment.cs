namespace Restaurant.Domain.Entities;

public sealed class Payment : BaseEntity
{
    public long OrderId { get; set; }
    public Order Order { get; set; }
    
    public decimal Amount { get; set; }
    
    public PaymentMethod Method { get; set; }
    
    public long WaiterId { get; set; }
    public User Waiter { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
}
public enum PaymentMethod
{
    Cash,
    Card
}

public enum PaymentStatus
{
    Pending,
    Paid
}