using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Inventories;

public static class InventoryErrors
{
    public static readonly Error NotFound = new(
        "Inventory.NotFound",
        "Inventory not found"
    );

    public static readonly Error AlreadyExists = new(
        "Inventory.AlreadyExists",
        "Inventory already exists for this dish"
    );

    public static readonly Error OutOfStock = new(
        "Inventory.OutOfStock",
        "Dish out of stock"
    );
}