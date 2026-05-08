using Restaurant.Application.Features.Orders.Models;

public class OrderDto
{
    public long Id { get; set; }
    public long TableId { get; set; }
    public long WaiterId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }

    public List<OrderItemDto> Items { get; set; } = new();
}