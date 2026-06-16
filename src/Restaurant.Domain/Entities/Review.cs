namespace Restaurant.Domain.Entities;

public class Review : BaseEntity
{
    public long OrderId { get; set; }
    public Order Order { get; set; }

    public long TableId { get; set; }
    public Table Table { get; set; }

    public ReviewType ReviewType { get; set; }

    public Grade Grade { get; set; }

    public string? Comment { get; set; }
}

public enum ReviewType
{
    Service = 1,
    Dish = 2,
    Table = 3,
    Cleanliness = 4,
    Restaurant = 5,
}

public enum Grade
{
    VeryBad = 1,
    Bad = 2,
    Average = 3,
    Good = 4,
    Excellent = 5
}