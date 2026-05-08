using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderItemByOrderIdAndDishIdSpec : Specification<OrderItem>
{
    public long OrderId { get; set; }
    public long DishId { get; set; }

    public OrderItemByOrderIdAndDishIdSpec(long orderId, long dishId, bool asNoTracking = false)
    {
        OrderId = orderId;
        DishId = dishId;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x =>
            x.OrderId == orderId &&
            x.DishId == dishId);
    }
}