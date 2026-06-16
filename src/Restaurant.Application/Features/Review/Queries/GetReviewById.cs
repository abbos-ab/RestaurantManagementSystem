using FluentValidation;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Review.Queries;

public sealed record GetReviewById(long ReviewId) : IQuery<ReviewDto>;

// ReSharper disable once UnusedType.Global
public sealed class GerReviewByIdValidator : AbstractValidator<GetReviewById>
{
    public GerReviewByIdValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0)
            .WithMessage("Review id must be greater than zero");
    }
}

internal sealed class GetReviewByIdHandler : IQueryHandler<GetReviewById, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewMapper _mapper;

    public GetReviewByIdHandler(IReviewRepository reviewRepository, ReviewMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(GetReviewById request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            throw new BusinessLogicException(ReviewErrors.NotFound);

        return _mapper.Map(review);
    }
}
