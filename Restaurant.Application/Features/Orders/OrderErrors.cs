using Restaurant.Shared.Common.Models;

namespace Restaurant.Application.Features.Orders;

public static class OrderErrors
{
    public static readonly Error NotFound = new(
        "Order.NotFound",
        "Order not found"
    );

    public static readonly Error CannotDeleteCompletedOrder = new(
        "Order.CannotDeleteCompletedOrder",
        "Cannot delete completed order"
    );
}