namespace Restaurant.Application.Features.Tables.Models;

public sealed class TableDto
{
    public long Id { get; set; }

    public int Number { get; set; }

    public int Capacity { get; set; }
    public DateTime CreatedAt { get; set; }
}