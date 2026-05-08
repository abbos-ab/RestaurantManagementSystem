namespace Restaurant.Application.Features.Dishes.Models;

public class DishDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long CategoryId { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}