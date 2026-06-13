using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Medias.Specifications;

public sealed class DishMediaIdsByDishIdSpec : Specification<DishMediaRelation, long>
{
    public long DishId { get; init; }

    public DishMediaIdsByDishIdSpec(long dishId)
    {
        DishId = dishId;

        Query
            .Where(x => x.DishId == dishId)
            .Select(x => x.MediaId);
    }
}