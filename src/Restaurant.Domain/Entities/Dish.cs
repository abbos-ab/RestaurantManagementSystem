namespace Restaurant.Domain.Entities
{
    public class Dish : BaseEntity
    {
        public string Name { get; set; }

        public long? CategoryId { get; set; }
        public Category? Category { get; set; }

        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}