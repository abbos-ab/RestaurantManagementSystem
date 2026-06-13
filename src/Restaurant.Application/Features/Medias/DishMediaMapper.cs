using Restaurant.Application.Features.Medias.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Medias;

[Mapper]
public partial class DishMediaMapper
{
    public partial DishMediaDto Map(DishMedia entity);
    
    public partial List<DishMediaDto> Map(List<DishMedia> entity);
}