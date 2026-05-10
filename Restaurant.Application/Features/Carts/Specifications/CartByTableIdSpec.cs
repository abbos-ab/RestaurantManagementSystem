using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Carts.Specifications;

public class CartByTableIdSpec : Specification<Cart>
{
    public long TableId { get; set; }

    public CartByTableIdSpec(long tableId, bool asNoTracking = false)
    {
        TableId = tableId;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.TableId == tableId);
    }
}