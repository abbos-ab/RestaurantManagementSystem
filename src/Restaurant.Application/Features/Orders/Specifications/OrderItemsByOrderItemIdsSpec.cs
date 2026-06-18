using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderItemsByOrderItemIdsSpec : Specification<OrderItem>
{
    public List<long> OrderItemIds { get; set; }
    
    public OrderItemsByOrderItemIdsSpec(List<long> orderItemIds, bool asNoTracking = false)
    {
        OrderItemIds = orderItemIds;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => orderItemIds.Contains(x.Id));
    }
}