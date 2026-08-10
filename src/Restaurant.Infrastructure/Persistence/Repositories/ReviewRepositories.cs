using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class ReviewRepository
    : RepositoryBase<Review>, IReviewRepository
{
    private readonly AppDbContext _context;


    public ReviewRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }


    public async Task<int> GetTodayReviewsCountAsync(
        DateTime today,
        CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .CountAsync(
                x => x.CreatedAt >= today,
                cancellationToken);
    }


    public async Task<double> GetAverageRatingAsync(
        CancellationToken cancellationToken)
    {
        var hasReviews = await _context.Reviews
            .AnyAsync(cancellationToken);

        if (!hasReviews)
            return 0;


        return await _context.Reviews
            .AverageAsync(
                x => (int)x.Grade,
                cancellationToken);
    }
}