using FluentValidation;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Review.Commands;

public record DeleteReviewCommand(long ReviewId) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0)
            .WithMessage("ReviewId must be greater than 0");
    }
}

internal class DeleteReviewCommandHandler : ICommandHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _reviewRepository;

    public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            throw new BusinessLogicException(ReviewErrors.NotFound);

        await _reviewRepository.DeleteAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}