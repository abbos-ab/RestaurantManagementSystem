using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Carts.Specifications;

public class CartByTableIdAndCartIdSpec : Specification<Cart>
{
    public long TableId { get; set; }
    public long CartId { get; set; }

    public CartByTableIdAndCartIdSpec(long tableId, long cartId, bool asNoTracking = false)
    {
        TableId = tableId;
        CartId = cartId;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.TableId == tableId && x.Id == cartId);
    }
}