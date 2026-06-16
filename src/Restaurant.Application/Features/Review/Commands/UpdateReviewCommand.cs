using FluentValidation;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Review.Commands;

public sealed record UpdateReviewCommand(
    long ReviewId,
    ReviewType ReviewType,
    Grade Grade,
    string Comment
) : ICommand<ReviewDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(r => r.ReviewId)
            .GreaterThan(0)
            .WithMessage("The review id must be greater than 0");

        RuleFor(r => r.ReviewType).IsInEnum();
        RuleFor(r => r.Grade).IsInEnum();
    }
}

internal sealed class UpdateReviewCommandHandler : ICommandHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly ReviewMapper _mapper;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository, ReviewMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            throw new BusinessLogicException(ReviewErrors.NotFound);

        if (request.ReviewType == review.ReviewType &&
            request.Grade == review.Grade &&
            request.Comment == review.Comment)
            throw new BusinessLogicException(ReviewErrors.NoChangesDetected);

        review.ReviewType = request.ReviewType;
        review.Grade = request.Grade;
        review.Comment = request.Comment;

        await _reviewRepository.UpdateAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map(review);
    }
}