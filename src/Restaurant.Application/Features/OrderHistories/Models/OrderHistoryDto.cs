namespace Restaurant.Application.Features.OrderHistories.Models;

public sealed class OrderHistoryDto
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public long? UserId { get; set; }

    public long? OrderItemId { get; set; }

    public DateTime CreatedAt { get; set; }
}