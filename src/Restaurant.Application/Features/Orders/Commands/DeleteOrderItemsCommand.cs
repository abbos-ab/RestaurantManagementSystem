using FluentValidation;
using MediatR;
using Restaurant.Application.Features.Notifications.Commands;
using Restaurant.Application.Features.Notifications.Events;
using Restaurant.Application.Features.OrderHistories.Events;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record DeleteOrderItemsCommand(long OrderId, List<long> OrderItemsIds) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteOrderItemsCommandValidator : AbstractValidator<DeleteOrderItemsCommand>
{
    public DeleteOrderItemsCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order Id must be greater than 0");

        RuleForEach(x => x.OrderItemsIds)
            .NotEmpty()
            .WithMessage("Order Items must not be empty");
    }
}

internal class DeleteOrderItemsCommandHandler : ICommandHandler<DeleteOrderItemsCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IMediator _mediator;

    public DeleteOrderItemsCommandHandler(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IMediator mediator)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteOrderItemsCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        List<OrderItem> orderItemsForDelete = new List<OrderItem>();
        
        var orderItemsSpec = new OrderItemsByOrderItemIdsSpec(request.OrderItemsIds);
        var orderItems = await _orderItemRepository.ListAsync(orderItemsSpec, cancellationToken);
        
        foreach (var item in request.OrderItemsIds)
        {
            var orderItem = orderItems.FirstOrDefault(x => x.Id == item);
            if (orderItem is null)
                throw new BusinessLogicException(OrderItemErrors.NotFound);

            if (orderItem.Status != OrderItemStatus.Preparing && orderItem.Status != OrderItemStatus.Pending)
                throw new BusinessLogicException(OrderItemErrors.OrderCompleted);

            orderItemsForDelete.Add(orderItem);
        }

        await _orderItemRepository.DeleteRangeAsync(orderItemsForDelete, cancellationToken);

        await _orderItemRepository.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new CreateNotificationEvent(
                order.WaiterId,
                NotificationType.OrderCancelled,
                order.Id,
                "Order cancelled"
            ),
            cancellationToken
        );

        await _mediator.Publish(new CreateOrderHistoryEvent(
                order.Id,
                OrderHistoryAction.Created,
                "Order created",
                order.WaiterId,
                null
            ),
            cancellationToken);
        
        return true;
    }
}