using Restaurant.Shared.Common.Models;

namespace Restaurant.Application.Features.Orders;

public static class OrderItemErrors
{
    public static readonly Error NotFound = new(
        "OrderItem.NotFound",
        "OrderItem not found"
    );
}