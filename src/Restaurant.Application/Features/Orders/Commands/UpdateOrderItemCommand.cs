using FluentValidation;
using MassTransit;
using MediatR;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Inventories.Specifications;
using Restaurant.Application.Features.OrderHistories.Events;
using Restaurant.Application.Features.Orders.Models;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Contracts.Events;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record UpdateOrderItemCommand(long OrderId, CreateOrderItemDto Item) : ICommand<OrderItemDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
{
    public UpdateOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order id must be greater than 0");

        RuleFor(x => x.Item)
            .SetValidator(new CreateOrderItemDtoValidator());
    }
}

internal sealed class UpdateOrderItemCommandHandler : ICommandHandler<UpdateOrderItemCommand, OrderItemDto>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IDishRepository _dishRepository;
    private readonly TimeProvider _timeProvider;
    private readonly OrderItemMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateOrderItemCommandHandler(
        IOrderItemRepository orderItemRepository,
        IOrderRepository orderRepository,
        IInventoryRepository inventoryRepository,
        IDishRepository dishRepository,
        TimeProvider timeProvider,
        OrderItemMapper mapper,
        IMediator mediator,
        IPublishEndpoint publishEndpoint)
    {
        _orderItemRepository = orderItemRepository;
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<OrderItemDto> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        var orderItemSpec = new OrderItemByDishIdSpec(request.OrderId, request.Item.DishId);
        var orderItem = await _orderItemRepository.FirstOrDefaultAsync(orderItemSpec, cancellationToken);
        if (orderItem is null)
            throw new BusinessLogicException(OrderItemErrors.NotFound);

        var inventorySpec = new InventoryByDishIdSpec(request.Item.DishId);
        var dishInventory = await _inventoryRepository.FirstOrDefaultAsync(inventorySpec, cancellationToken);
        if (dishInventory is null)
            throw new BusinessLogicException(InventoryErrors.NotFound);

        if (dishInventory.Quantity < request.Item.Quantity)
            throw new BusinessLogicException(InventoryErrors.OutOfStock);

        var dish = await _dishRepository.GetByIdAsync(request.Item.DishId, cancellationToken);
        if (dish is null)
            throw new BusinessLogicException(DishErrors.NotFound);

        var deference = orderItem.Quantity - request.Item.Quantity;

        if (orderItem.Status == OrderItemStatus.Pending || orderItem.Status == OrderItemStatus.Preparing)
        {
            orderItem.Quantity = request.Item.Quantity;
            orderItem.TotalPrice = dish.Price * request.Item.Quantity;
            orderItem.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

            dishInventory.Quantity += deference;
            order.TotalPrice -= dish.Price * deference;

            await _orderRepository.UpdateAsync(order, cancellationToken);
            await _orderItemRepository.UpdateAsync(orderItem, cancellationToken);
            await _inventoryRepository.UpdateAsync(dishInventory, cancellationToken);

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
                    OrderHistoryAction.ItemChanged,
                    "Order created",
                    order.WaiterId,
                    orderItem.Id
                ),
                cancellationToken);

            return _mapper.Map(orderItem);
        }
        else
        {
            throw new BusinessLogicException(OrderItemErrors.OrderCompleted);
        }
    }
}