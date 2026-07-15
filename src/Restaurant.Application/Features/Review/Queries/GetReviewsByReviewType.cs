using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Review.Queries;

public record GetReviewsByReviewType(ReviewType ReviewType) : IQuery<IEnumerable<ReviewDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetReviewsByReviewTypeValidator : AbstractValidator<GetReviewsByReviewType>
{
    public GetReviewsByReviewTypeValidator()
    {
        RuleFor(r => r.ReviewType)
            .IsInEnum()
            .WithMessage("Invalid review type");
    }
}

internal sealed class GetReviewsByReviewTypeHandler : IQueryHandler<GetReviewsByReviewType, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewMapper _reviewMapper;

    public GetReviewsByReviewTypeHandler(IReviewRepository reviewRepository, ReviewMapper reviewMapper)
    {
        _reviewRepository = reviewRepository;
        _reviewMapper = reviewMapper;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByReviewType request,
        CancellationToken cancellationToken)
    {
        var spec = new DbSpecification<Domain.Entities.Review>();
        spec.Query.Where(x => x.ReviewType == request.ReviewType);

        var reviews = await _reviewRepository.ListAsync(spec, cancellationToken);
        if (!reviews.Any())
            return [];

        return _reviewMapper.Map(reviews);
    }
}