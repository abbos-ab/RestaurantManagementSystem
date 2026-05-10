namespace Restaurant.Application.Features.Orders.Models;

public class OrderItemDto
{
    public long Id { get; set; }
    public long DishId { get; set; }
    public long Quantity { get; set; }
    public decimal Price { get; set; }
    public string Status { get; set; }
}