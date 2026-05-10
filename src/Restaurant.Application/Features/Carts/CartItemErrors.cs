using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Carts;

public static class CartItemErrors
{
    public static readonly Error NotFound = new(
        "CartItem.NotFound",
        "Cart item not found"
    );

    public static readonly Error AlreadyExists = new(
        "CartItem.AlreadyExists",
        "Cart item already exists"
    );
}