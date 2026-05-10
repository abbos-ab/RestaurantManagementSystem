namespace Restaurant.Application.Features.Carts.Models;

public class CartItemDto
{
    public long Id { get; set; }
    public long CartId { get; set; }
    public long DishId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}