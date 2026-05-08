using Restaurant.Application.Features.Dishes.Commands;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Dishes;


[Mapper]
public partial class DishMapper
{
    public partial DishDto Map(Dish dish);
    public partial List<DishDto> Map(List<Dish> dish);
    public partial Dish ToEntity(CreateDishCommand command);
}