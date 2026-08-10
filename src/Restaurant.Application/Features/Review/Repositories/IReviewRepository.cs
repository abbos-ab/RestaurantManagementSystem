using Ardalis.Specification;

namespace Restaurant.Application.Features.Review.Repositories;
    
public interface IReviewRepository : IRepositoryBase<Domain.Entities.Review>
{
    Task<int> GetTodayReviewsCountAsync(
    DateTime today,
    CancellationToken cancellationToken);

    Task<double> GetAverageRatingAsync(
        CancellationToken cancellationToken);
}