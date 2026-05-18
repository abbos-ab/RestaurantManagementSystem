using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Waitors;

public static class WaiterErrors
{
    public static readonly Error AlreadyTaken = new(
        "Waiter.AlreadyTaken",
        "This order has already been taken by another waiter."
    );
}