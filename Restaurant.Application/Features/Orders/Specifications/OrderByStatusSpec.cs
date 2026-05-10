using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public sealed class OrderByStatusSpec : Specification<Order>
{
    public OrderStatus Status  { get; set; }
    
    public OrderByStatusSpec(OrderStatus status, bool asNoTracking = false)
    {
        Status = status;
        
        if (asNoTracking)
            Query.AsNoTracking();
        
        Query.Where(o => o.Status == status);
    }
}