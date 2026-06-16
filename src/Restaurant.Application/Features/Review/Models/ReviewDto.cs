using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Review.Models;

public sealed class ReviewDto
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long TableId { get; set; }

    public ReviewType Type { get; set; }

    public Grade Grade { get; set; }
}