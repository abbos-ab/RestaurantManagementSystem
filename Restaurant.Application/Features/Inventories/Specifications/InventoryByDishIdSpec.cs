using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Inventories.Specifications;

public sealed class InventoryByDishIdSpec : Specification<Inventory>
{
    public long DishId { get; set; }

    public InventoryByDishIdSpec(long dishId, bool asNoTracking = false)
    {
        DishId = dishId;

        if (asNoTracking)
            Query.AsNoTracking();
        
        Query.Where(x => x.DishId == dishId);
    }
}