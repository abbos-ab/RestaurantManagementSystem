using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.OrderHistories;

public static class OrderHistoryErrors
{
    public static readonly Error NotFound = new(
        "OrderHistory.NotFound",
        "Order history not found"
    );

    public static readonly Error AlreadyExists = new(
        "OrderHistory.AlreadyExists",
        "Order history already exists"
    );
}