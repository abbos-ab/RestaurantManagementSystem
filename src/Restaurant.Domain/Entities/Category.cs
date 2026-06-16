namespace Restaurant.Domain.Entities;

public sealed class Category :  BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public List<Dish> Dishes { get; set; } = new List<Dish>();
}