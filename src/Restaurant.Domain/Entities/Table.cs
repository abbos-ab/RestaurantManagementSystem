namespace Restaurant.Domain.Entities
{
    public class Table : BaseEntity
    {
        public int Number { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}