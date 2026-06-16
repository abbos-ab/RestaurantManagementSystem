using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.Review;

public static class ReviewErrors
{
    public static readonly Error NotFound = new(
        "Review.NotFound",
        "Review not found"
    );

    public static readonly Error NoChangesDetected = new(
        "Review.NoChangesDetected",
        "No changes were detected."
    );
}