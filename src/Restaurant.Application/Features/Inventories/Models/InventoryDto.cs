namespace Restaurant.Application.Features.Inventories.Models;

public class InventoryDto
{
    public long Id { get; set; }
    public long DishId { get; set; }
    public string? DishName { get; set; }
    public int Quantity { get; set; }
}