namespace Restaurant.Application.Features.Orders.Models;

public class CreateOrderItemDto
{
    public long DishId { get; set; }
    public int Quantity { get; set; }
}