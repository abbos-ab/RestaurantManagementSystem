using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Orders;

public static class OrderItemErrors
{
    public static readonly Error NotFound = new(
        "OrderItem.NotFound",
        "OrderItem not found."
    );

    public static readonly Error OrderCompleted = new(
        "OrderItem.OrderCompleted",
        "Cannot modify order items because the order is completed."
    );
}