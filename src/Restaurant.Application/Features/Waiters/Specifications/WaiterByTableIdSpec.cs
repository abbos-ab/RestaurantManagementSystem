using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Waiters.Specifications;

public sealed class WaiterByTableIdSpec : Specification<Order, long?>
{
    public long TableId { get; set; }

    public WaiterByTableIdSpec(long tableId, bool asNoTracking = false)
    {
        TableId = tableId;

        if (asNoTracking)
            Query.AsNoTracking();

        Query
            .Where(x => x.TableId == tableId)
            .Select(order => order.WaiterId);
    }
}