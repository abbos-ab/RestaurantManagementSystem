using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderItemByDishIdSpec : Specification<OrderItem>
{
    public long DishId { get; set; }
    
    public OrderItemByDishIdSpec(long dishId, bool asNoTracking = false)
    {
        DishId = dishId;
        
        if (asNoTracking)
            Query.AsNoTracking();
        
        Query.Where(x => x.DishId == dishId);
    }
}