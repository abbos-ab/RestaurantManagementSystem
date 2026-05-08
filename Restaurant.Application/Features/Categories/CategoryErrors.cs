using Restaurant.Shared.Common.Models;

namespace Restaurant.Application.Features.Categories;

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound",
        "Category not found"
    );

    public static readonly Error AlreadyExists = new(
        "Category.AlreadyExists",
        "Category already exists"
    );

    public static readonly Error HasRelatedDishes = new(
        "Category.HasRelatedDishes",
        "Category has related dishes and cannot be deleted"
    );
}