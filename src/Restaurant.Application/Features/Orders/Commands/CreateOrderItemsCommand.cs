using FluentValidation;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Inventories.Specifications;
using Restaurant.Application.Features.Orders.Models;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record CreateOrderItemsCommand(long OrderId, List<CreateOrderItemDto> Items)
    : ICommand<bool>;

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

    public CreateOrderItemCommandHandler(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        TimeProvider timeProvider)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _dishRepository = dishRepository;
        _inventoryRepository = inventoryRepository;
        _timeProvider = timeProvider;
    }

    public async Task<bool> Handle(CreateOrderItemsCommand request, CancellationToken cancellationToken)
    {
        bool isDiscountable = DateTime.Now.Hour > 0 && DateTime.Now.Hour < 8;

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        decimal total = 0;

        foreach (var item in request.Items)
        {
            var dish = await _dishRepository.GetByIdAsync(item.DishId, cancellationToken);

            if (dish is null)
                throw new BusinessLogicException(DishErrors.NotFound);

            var inventoryByDishIdSpec = new InventoryByDishIdSpec(item.DishId);
            var dishInventory =
                await _inventoryRepository.FirstOrDefaultAsync(inventoryByDishIdSpec, cancellationToken);

            if (dishInventory is null)
                throw new BusinessLogicException(InventoryErrors.NotFound);

            if (dishInventory.Quantity < item.Quantity)
                throw new ResourceNotFoundException(InventoryErrors.OutOfStock);

            var orderItemByDishIdSpec = new OrderItemByOrderIdAndDishIdSpec(request.OrderId, item.DishId);
            var orderItem = await _orderItemRepository.FirstOrDefaultAsync(orderItemByDishIdSpec, cancellationToken);

            if (orderItem is null)
            {
                var newOrderItem = new OrderItem
                {
                    OrderId = order.Id,
                    DishId = item.DishId,
                    Quantity = item.Quantity,
                    TotalPrice = isDiscountable ? ((dish.Price * item.Quantity) / 100) * 80 : dish.Price * item.Quantity,
                    Status = OrderItemStatus.Pending,
                    CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
                };

                total += item.Quantity * dish.Price;

                dishInventory.Quantity -= item.Quantity;
                await _orderItemRepository.AddAsync(newOrderItem, cancellationToken);
            }
            else
            {
                orderItem.Quantity += item.Quantity;
                orderItem.TotalPrice +=
                    isDiscountable ? ((dish.Price * item.Quantity) / 100) * 80 : dish.Price * item.Quantity;
                orderItem.Status = OrderItemStatus.Pending;

                total += item.Quantity * dish.Price;

                dishInventory.Quantity -= item.Quantity;
                await _orderItemRepository.UpdateAsync(orderItem, cancellationToken);
            }
        }

        order.TotalPrice += total;

        await _inventoryRepository.SaveChangesAsync(cancellationToken);
        await _orderItemRepository.SaveChangesAsync(cancellationToken);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}