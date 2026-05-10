using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Inventories;

[Mapper]
public partial class InventoryMapper
{
    public partial InventoryDto Map(Inventory inventory);
    public partial List<InventoryDto> Map(List<Inventory> inventories);
}