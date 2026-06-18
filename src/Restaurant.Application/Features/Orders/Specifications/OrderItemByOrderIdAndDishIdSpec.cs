using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderItemByOrderIdAndDishIdSpec : Specification<OrderItem>
{
    public long OrderId { get; set; }
    public List<long> DishIds { get; set; }

    public OrderItemByOrderIdAndDishIdSpec(long orderId, List<long> dishIds, bool asNoTracking = false)
    {
        OrderId = orderId;
        DishIds = dishIds;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.OrderId == orderId && dishIds.Contains(x.DishId));
    }
}