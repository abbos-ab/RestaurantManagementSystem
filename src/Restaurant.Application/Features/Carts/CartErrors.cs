using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Carts;

public static class CartErrors
{
    public static readonly Error NotFound = new(
        "Cart.NotFound",
        "Cart not found"
    );

    public static readonly Error AlreadyExists = new(
        "Cart.AlreadyExists",
        "Cart already exists"
    );
}