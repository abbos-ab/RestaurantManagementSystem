    namespace Restaurant.Domain.Entities;

    public sealed class Dish : BaseEntity
    {
        public const string DishMinioFolder = "dishes";
        public string Name { get; set; }

        public long? CategoryId { get; set; }
        public Category? Category { get; set; }

        public decimal Price { get; set; }

        public long? MediaPicId { get; set; }
        public DishMedia? MediaPic { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }