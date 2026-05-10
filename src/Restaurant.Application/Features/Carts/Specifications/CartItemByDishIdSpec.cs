using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Carts.Specifications;

public class CartItemByDishIdSpec : Specification<CartItem>
{
    public long CartId { get; set; }
    public long DishId { get; set; }

    public CartItemByDishIdSpec(long cartId, long dishId, bool asNoTracking = false)
    {
        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.CartId == cartId && x.DishId == dishId);
    }
}