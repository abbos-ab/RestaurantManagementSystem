namespace Restaurant.Domain.Entities;

public sealed class Inventory : BaseEntity
{
    public long DishId { get; set; }
    public Dish Dish { get; set; }

    public int Quantity { get; set; }
}