using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderItemByDishIdSpec : Specification<OrderItem>
{
    public long OrderId { get; set; }
    public long DishId { get; set; }
    
    public OrderItemByDishIdSpec(long orderId,long dishId, bool asNoTracking = false)
    {
        OrderId = orderId;
        DishId = dishId;
        
        if (asNoTracking)
            Query.AsNoTracking();
        
        Query.Where(x => x.OrderId == OrderId && x.DishId == DishId);
    }
}