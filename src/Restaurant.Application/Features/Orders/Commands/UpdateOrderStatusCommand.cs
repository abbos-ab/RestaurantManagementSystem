using FluentValidation;
using MassTransit;
using MediatR;
using Restaurant.Application.Features.OrderHistories.Events;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Contracts.Events;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(
    long OrderId,
    OrderStatus Status
) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");

        RuleFor(x => x.Status).IsInEnum();
    }
}

internal sealed class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        TimeProvider timeProvider,
        IMediator mediator,
        IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _timeProvider = timeProvider;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        order.Status = request.Status;
        order.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _orderRepository.UpdateAsync(order, cancellationToken);

        await _publishEndpoint.Publish(new OrderUpdatedEvent
        {
            OrderId = order.Id,
            UserId = order.WaiterId,
            TotalAmount = order.TotalPrice,
            UpdateDescription = "Order items were updated.",
            UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        }, cancellationToken);

        await _mediator.Publish(new CreateOrderHistoryEvent(
                order.Id,
                OrderHistoryAction.StatusChanged,
                "Order status changed",
                order.WaiterId,
                null
            ),
            cancellationToken);

        return true;
    }
}