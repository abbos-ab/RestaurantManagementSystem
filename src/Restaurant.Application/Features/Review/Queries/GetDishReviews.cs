using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Review.Queries;

public record GetDishReviews(long DishId) : IQuery<IEnumerable<ReviewDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetDishReviewsValidator : AbstractValidator<GetDishReviews>
{
    public GetDishReviewsValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("Dish id must be greater than 0");
    }
}

internal sealed class GetDishReviewsHandler : IQueryHandler<GetDishReviews, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewMapper _reviewMapper;

    public GetDishReviewsHandler(IReviewRepository reviewRepository, ReviewMapper reviewMapper)
    {
        _reviewRepository = reviewRepository;
        _reviewMapper = reviewMapper;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetDishReviews request, CancellationToken cancellationToken)
    {
        var spec = new DbSpecification<Domain.Entities.Review>();
        spec.Query.Where(x => x.DishId == request.DishId);

        var review = await _reviewRepository.ListAsync(spec, cancellationToken);
        if (!review.Any())
            return [];

        return _reviewMapper.Map(review);
    }
}

//TODO add migration for review entity
