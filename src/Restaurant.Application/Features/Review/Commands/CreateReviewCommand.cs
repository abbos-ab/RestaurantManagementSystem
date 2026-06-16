using FluentValidation;
using Restaurant.Application.Features.Orders;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Review.Models;
using Restaurant.Application.Features.Review.Repositories;
using Restaurant.Application.Features.Tables;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Review.Commands;

public sealed record CreateReviewCommand(
    long OrderId,
    long TableId,
    ReviewType ReviewType,
    Grade Grade,
    string Comment
) : ICommand<ReviewDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ReviewType).IsInEnum();
        RuleFor(x => x.Grade).IsInEnum();

        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than zero");

        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than zero");
    }
}

internal sealed class CreateReviewCommandHandler : ICommandHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ReviewMapper _mapper;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IOrderRepository orderRepository,
        ITableRepository tableRepository,
        TimeProvider timeProvider,
        ReviewMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new ResourceNotFoundException(OrderErrors.NotFound);

        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);
        if (table is null)
            throw new ResourceNotFoundException(TableErrors.NotFound);

        var review = new Domain.Entities.Review
        {
            OrderId = order.Id,
            TableId = table.Id,
            ReviewType = request.ReviewType,
            Grade = request.Grade,
            Comment = request.Comment,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map(review);
    }
}