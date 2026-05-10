using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Specifications;

public class OrderByTableIdAndOrderId : Specification<Order>
{
    public long TableId { get; set; }
    public long OrderId { get; set; }

    public OrderByTableIdAndOrderId(long tableId, long orderId, bool asNoTracking = false)
    {
        TableId = tableId;
        OrderId = orderId;
        
        if (asNoTracking)
            Query.AsNoTracking();

        Query
            .Where(x =>
            x.TableId == tableId &&
            x.Id == orderId);
    }
}