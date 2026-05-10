namespace Restaurant.Application.Features.Orders.Models;

public sealed record DeleteOrderRequest
{
    public long TableId { get; set; }
    public long OrderId { get; set; }
}