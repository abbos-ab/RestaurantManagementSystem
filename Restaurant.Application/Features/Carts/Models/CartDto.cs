namespace Restaurant.Application.Features.Carts.Models;

public class CartDto
{
    public long Id { get; set; }
    public long TableId { get; set; }
    public decimal TotalPrice { get; set; }

    public List<CartItemDto> CartItems { get; set; } = new();
}