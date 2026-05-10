using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Dishes.Specifications;

public sealed class DishByNameSpec : Specification<Dish>
{
    public string DishName { get; set; }

    public DishByNameSpec(string dishName, bool asNoTracking = false)
    {
        DishName = dishName;
        
        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.Name == dishName);
    }
}