namespace Restaurant.Domain.Entities
{
    public class Table : BaseEntity
    {
        public int Number { get; set; }

        public TableStatus Status { get; set; }

        public int Capacity { get; set; }

        public List<Order> Orders { get; set; } = new();
    }

    public enum TableStatus
    {
        Available,    
        Occupied,     
        Reserved,     
        Cleaning,     
        Disabled
    }
}