namespace Restaurant.Domain.Entities;

public sealed class DishMediaRelation
{
    public required long DishId { get; set; }
    public Dish Dish { get; set; } = null!;

    public required long MediaId { get; set; }
    public DishMedia Media { get; set; } = null!;
}