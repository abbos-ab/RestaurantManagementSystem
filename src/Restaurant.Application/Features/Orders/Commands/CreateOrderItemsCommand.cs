using FluentValidation;
using MassTransit;
using MediatR;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Dishes.Specifications;
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

public sealed record CreateOrderItemsCommand(
    long OrderId,
    List<CreateOrderItemDto> Items
) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemsCommand>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemDtoValidator());
    }
}

internal sealed class CreateOrderItemCommandHandler : ICommandHandler<CreateOrderItemsCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IMediator _mediator;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderItemCommandHandler(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        TimeProvider timeProvider,
        IMediator mediator,
        IPublishEndpoint publishEndpoint)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _dishRepository = dishRepository;
        _inventoryRepository = inventoryRepository;
        _timeProvider = timeProvider;
        _mediator = mediator;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
    {
        bool isDiscountable = DateTime.Now.Hour > 0 && DateTime.Now.Hour < 8;

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        decimal total = 0;
        List<OrderItem> newOrderItems = new List<OrderItem>();
        List<OrderItem> oldOrderItems = new List<OrderItem>();

        var dishIds = request.Items.Select(c => c.DishId).ToList();

        var dishesSpec = new DishesByDishIdsSpec(dishIds);
        var inventoriesSpec = new InventoriesByDishIdsSpec(dishIds);
        var orderItemsSpec = new OrderItemByOrderIdAndDishIdSpec(request.OrderId, dishIds);

        var dishes = await _dishRepository.ListAsync(dishesSpec, cancellationToken);
        var inventories = await _inventoryRepository.ListAsync(inventoriesSpec, cancellationToken);
        var orderItems = await _orderItemRepository.ListAsync(orderItemsSpec, cancellationToken);

        foreach (var item in request.Items)
        {
            var dish = dishes.FirstOrDefault(x => x.Id == item.DishId);

            if (dish is null)
                throw new BusinessLogicException(DishErrors.NotFound);

            var dishInventory = inventories.FirstOrDefault(x => x.DishId == item.DishId);

            if (dishInventory is null)
                throw new BusinessLogicException(InventoryErrors.NotFound);

            if (dishInventory.Quantity < item.Quantity)
                throw new ResourceNotFoundException(InventoryErrors.OutOfStock);

            var orderItem = orderItems.FirstOrDefault(x => x.DishId == item.DishId);

            if (orderItem is null)
            {
                var newOrderItem = new OrderItem
                {
                    OrderId = order.Id,
                    DishId = item.DishId,
                    Quantity = item.Quantity,
                    TotalPrice =
                        isDiscountable ? ((dish.Price * item.Quantity) / 100) * 80 : dish.Price * item.Quantity,
                    Status = OrderItemStatus.Pending,
                    CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
                };

                total += item.Quantity * dish.Price;

                dishInventory.Quantity -= item.Quantity;
                newOrderItems.Add(newOrderItem);
            }
            else
            {
                orderItem.Quantity += item.Quantity;
                orderItem.TotalPrice +=
                    isDiscountable ? ((dish.Price * item.Quantity) / 100) * 80 : dish.Price * item.Quantity;
                orderItem.Status = OrderItemStatus.Pending;

                total += item.Quantity * dish.Price;

                dishInventory.Quantity -= item.Quantity;
                oldOrderItems.Add(orderItem);
            }
        }

        order.TotalPrice += total;

        await _inventoryRepository.SaveChangesAsync(cancellationToken);
        await _orderItemRepository.AddRangeAsync(newOrderItems, cancellationToken);
        await _orderItemRepository.UpdateRangeAsync(oldOrderItems, cancellationToken);
        await _orderItemRepository.SaveChangesAsync(cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new OrderPlacedEvent
        {
            OrderId = order.Id,
            UserId = order.WaiterId,
            CustomerName = "TEST orderItemCreate",
            TotalAmount = order.TotalPrice
        }, cancellationToken);

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