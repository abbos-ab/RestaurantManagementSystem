using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Dishes.Specifications;

public class DishesByDishIdsSpec : Specification<Dish>
{
    public List<long> DishIds { get; set; }

    public DishesByDishIdsSpec(List<long> dishIds, bool asNoTracking = false)
    {
        DishIds = dishIds;
        
        if(asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => dishIds.Contains(x.Id));
    }
}