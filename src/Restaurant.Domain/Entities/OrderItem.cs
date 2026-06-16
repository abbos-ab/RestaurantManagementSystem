namespace Restaurant.Domain.Entities;
   
public sealed class OrderItem : BaseEntity
{
    public long OrderId { get; set; }
    public Order Order { get; set; }

    public long DishId { get; set; }
    public Dish Dish { get; set; }
        
    public long? ChefId { get; set; }
    public User? Chef { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;

    public DateTime? PreparedAt { get; set; }
    public DateTime? ServedAt { get; set; }
        
    public string? CancelReason { get; set; }
}

public enum OrderItemStatus
{
    Pending,       
    Preparing,     
    Ready,         
    Served,        
    Cancelled      
}