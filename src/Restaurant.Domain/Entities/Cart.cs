namespace Restaurant.Domain.Entities;

public class Cart : BaseEntity
{
    public long TableId { get; set; }
    public Table Table { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    public List<CartItem> CartItems { get; set; }
}