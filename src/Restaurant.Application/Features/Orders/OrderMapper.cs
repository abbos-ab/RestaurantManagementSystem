using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Orders;

[Mapper]
public partial class OrderMapper
{
    public partial OrderDto Map(Order order);
    public partial List<OrderDto> Map(List<Order> orders);
}