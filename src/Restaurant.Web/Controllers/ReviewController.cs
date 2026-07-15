using MediatR;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.Features.Review.Commands;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Queries;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Web.Models;

namespace Restaurant.Web.Controllers;

public class ReviewController : BaseController
{
    public ReviewController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<PaginatedResult<ReviewDto>> GetAll(
        [FromBody] PaginationInfo paginationInfo,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetAllReviews(paginationInfo), cancellationToken);
    }

    [HttpGet("{ReviewId:long}")]
    public async Task<ReviewDto> Get(
        long reviewId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetReviewById(reviewId), cancellationToken);
    }

    [HttpPost]
    public async Task<ReviewDto> CreateReview(
        [FromBody] CreateReviewCommand command,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    [HttpPut("{ReviewId:long}")]
    public async Task<ReviewDto> UpdateReview(
        long reviewId,
        [FromBody] UpdateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new UpdateReviewCommand(
                reviewId,
                request.ReviewType,
                request.Grade,
                request.Comment),
            cancellationToken
        );
    }

    [HttpDelete("{ReviewId:long}")]
    public async Task<bool> DeleteReview(
        long reviewId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new DeleteReviewCommand(reviewId), cancellationToken);
    }

    [HttpGet]
    public async Task<IEnumerable<ReviewDto>> GetReviewsByDishId(
        long dishId,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetDishReviews(dishId), cancellationToken);
    }

    [HttpGet]
    public async Task<IEnumerable<ReviewDto>> GetReviewsByReviewType(
        ReviewType reviewType,
        CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetReviewsByReviewType(reviewType), cancellationToken);
    }
}