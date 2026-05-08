using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderByTableIdSpec : Specification<Order>
{
    public long TableId { get; }

    public OrderByTableIdSpec(long tableId, bool asNoTracking = true)
    {
        TableId = tableId;
        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(o => o.TableId == tableId);
    }
}