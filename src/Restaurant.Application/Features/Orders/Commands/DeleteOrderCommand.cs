using FluentValidation;
using MassTransit;
using MediatR;
using Restaurant.Application.Features.OrderHistories.Events;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Contracts.Events;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record DeleteOrderCommand(long TableId, long OrderId) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");

        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");
    }
}

internal sealed class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly TimeProvider _timeProvider;

    public DeleteOrderCommandHandler(
        IOrderRepository orderRepository,
        IMediator mediator,
        IPublishEndpoint publishEndpoint,
        TimeProvider timeProvider)
    {
        _orderRepository = orderRepository;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
        _timeProvider = timeProvider;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderByTableIdAndOrderIdSpec = new OrderByTableIdAndOrderId(request.TableId, request.OrderId);
        var order = await _orderRepository.FirstOrDefaultAsync(orderByTableIdAndOrderIdSpec, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        if (order.Status == OrderStatus.Completed)
            throw new BusinessLogicException(OrderErrors.CannotDeleteCompletedOrder);

        await _mediator.Publish(new CreateOrderHistoryEvent(
                OrderId: order.Id,
                OrderHistoryAction.Cancelled,
                "Order cancelled",
                UserId: order.WaiterId,
                null
            ),
            cancellationToken);

        await _publishEndpoint.Publish(new OrderCancelledEvent
        {
            OrderId = order.Id,
            UserId = order.WaiterId,
            Reason = "Customer changed mind",
            CancelledAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        }, cancellationToken);

        await _orderRepository.DeleteAsync(order, cancellationToken);

        return true;
    }
}