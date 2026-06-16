using Restaurant.Domain.Entities;

namespace Restaurant.Web.Models;

public sealed record UpdateReviewRequest(
    ReviewType ReviewType,
    Grade Grade,
    string Comment
);