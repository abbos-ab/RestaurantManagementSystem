using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Inventories.Specifications;

public class InventoriesByDishIdsSpec : Specification<Inventory>
{
    public List<long> DishIds { get; }

    public InventoriesByDishIdsSpec(List<long> dishIds, bool asNoTracking = false)
    {
        DishIds = dishIds;
        
        if(asNoTracking)
            Query.AsNoTracking();
        
        Query.Where(x => dishIds.Contains(x.DishId));
    }
}