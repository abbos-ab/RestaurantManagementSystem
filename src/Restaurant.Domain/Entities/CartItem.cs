namespace Restaurant.Domain.Entities;

public sealed class CartItem : BaseEntity
{
    public long CartId { get; set; }
    public Cart Cart { get; set; }

    public long DishId { get; set; }
    public Dish Dish { get; set; }

    public decimal Price { get; set; }
    public int Quantity { get; set; }
}