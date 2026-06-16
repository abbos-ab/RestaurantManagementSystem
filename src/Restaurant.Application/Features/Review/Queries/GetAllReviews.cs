using Ardalis.Specification;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Review.Queries;

public record GetAllReviews(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<ReviewDto>>;

internal sealed class GetAllReviewsHandler : IQueryHandler<GetAllReviews, PaginatedResult<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewMapper _mapper;

    public GetAllReviewsHandler(IReviewRepository reviewRepository, ReviewMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ReviewDto>> Handle(GetAllReviews request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Domain.Entities.Review>();
        spec.Query
            .Include(x => x.Order)
            .Include(x => x.Table)
            .WithPagination(request.PaginationInfo);

        var reviews = await _reviewRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _reviewRepository.CountAsync(spec, cancellationToken);

        var mapperReview = _mapper.Map(reviews);

        return new PaginatedResult<ReviewDto>(mapperReview, totalCount);
    }
}