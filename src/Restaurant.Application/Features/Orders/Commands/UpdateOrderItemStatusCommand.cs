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

public sealed record UpdateOrderItemStatusCommand(
    long OrderItemId,
    OrderItemStatus Status
) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public class UpdateOrderItemStatusCommandValidator : AbstractValidator<UpdateOrderItemStatusCommand>
{
    public UpdateOrderItemStatusCommandValidator()
    {
        RuleFor(x => x.OrderItemId)
            .GreaterThan(0)
            .WithMessage("OrderItemId must be greater than 0");
    }
}

internal sealed class UpdateOrderItemStatusCommandHandler : ICommandHandler<UpdateOrderItemStatusCommand, bool>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateOrderItemStatusCommandHandler(
        IOrderItemRepository orderItemRepository,
        TimeProvider timeProvider,
        IMediator mediator,
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint)
    {
        _orderItemRepository = orderItemRepository;
        _timeProvider = timeProvider;
        _mediator = mediator;
        _orderRepository = orderRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(UpdateOrderItemStatusCommand request, CancellationToken cancellationToken)
    {
        var item = await _orderItemRepository.GetByIdAsync(request.OrderItemId, cancellationToken);

        if (item is null)
            throw new BusinessLogicException(OrderItemErrors.NotFound);

        item.Status = request.Status;
        item.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        if (request.Status == OrderItemStatus.Ready)
            item.PreparedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        if (request.Status == OrderItemStatus.Served)
            item.ServedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        var order = await _orderRepository.GetByIdAsync(item.OrderId, cancellationToken);

        await _orderItemRepository.UpdateAsync(item, cancellationToken);

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
                "Order item status changed",
                order.WaiterId,
                null
            ),
            cancellationToken);

        return true;
    }
}