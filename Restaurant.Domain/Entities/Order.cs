namespace Restaurant.Domain.Entities
{
    public class Order : BaseEntity
    {
        public long TableId { get; set; }
        public Table Table { get; set; }

        public long WaiterId { get; set; }
        public User Waiter { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Created;

        public decimal TotalPrice { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        Created,
        InProgress,
        Completed,
        Rejected,
    }
}