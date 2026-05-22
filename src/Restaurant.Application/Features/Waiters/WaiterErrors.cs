using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Waiters;

public static class WaiterErrors
{
    public static readonly Error NotFound = new(
        "Waiter.NotFound",
        "Waiter not found."
    );

    public static readonly Error AlreadyTaken = new(
        "Waiter.AlreadyTaken",
        "This order is taken by another waiter."
    );

    public static readonly Error OrderNotFound = new(
        "Waiter.OrderNotFound",
        "Order not found."
    );

    public static readonly Error OrderCompleted = new(
        "Waiter.OrderCompleted",
        "This order is completed."
    );

    public static readonly Error OrderRejected = new(
        "Waiter.OrderRejected",
        "This order is rejected."
    );

    public static readonly Error AlreadyAssigned = new(
        "Waiter.AlreadyAssigned",
        "This order is already assigned to this waiter."
    );
}