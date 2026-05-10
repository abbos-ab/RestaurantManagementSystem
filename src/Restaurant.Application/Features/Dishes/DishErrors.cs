using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Dishes;

public static class DishErrors
{
    public static readonly Error NotFound = new(
        "Dish.NotFound",
        "Dish not found"
    );

    public static readonly Error AlreadyExists = new(
        "Dish.AlreadyExists",
        "Dish already exists"
    );
}