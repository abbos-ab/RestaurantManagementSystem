using Restaurant.Application.Features.Orders.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Orders;

[Mapper]
public partial class OrderItemMapper
{
    public partial OrderItemDto Map(OrderItem orderItem);
    public partial List<OrderItemDto> Map(List<Order> orderItems);
}