using Restaurant.Application.Features.Review.Models;
using Riok.Mapperly.Abstractions;

namespace Restaurant.Application.Features.Review;

[Mapper]
public partial class ReviewMapper
{
    public partial ReviewDto Map(Domain.Entities.Review review);

    public partial List<ReviewDto> Map(List<Domain.Entities.Review> reviews);
}