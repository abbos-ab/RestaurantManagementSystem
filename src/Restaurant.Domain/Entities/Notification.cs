namespace Restaurant.Domain.Entities;

public sealed class Notification : BaseEntity
{
    public long UserId { get; set; }
    public User User { get; set; }
    
    public NotificationType Type { get; set; } 

    public long? OrderId { get; set; }
    public Order? Order { get; set; }
    
    public string? Message { get; set; }

    public bool IsRead { get; set; } = false;
}

public enum NotificationType
{
    // Order
    OrderCreated,         
    OrderUpdated,         
    OrderCancelled,       
    
    // Prepare
    OrderPreparing,       
    OrderReady,           
    
    // Service
    OrderServed,          
    
    // Table
    TableCalledWaiter,    
    TableChanged,         
    
    // Payment
    PaymentRequested,     
    PaymentCompleted,     
}