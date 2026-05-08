namespace Restaurant.Domain.Entities;

public class Category :  BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public List<Dish> Dishes { get; set; } = new List<Dish>();
}