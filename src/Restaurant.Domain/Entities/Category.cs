namespace Restaurant.Domain.Entities;

public class Category :  BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    
    public List<Dish> Dishes { get; set; } = new List<Dish>();
}