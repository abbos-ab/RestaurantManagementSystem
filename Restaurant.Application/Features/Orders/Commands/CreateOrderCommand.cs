using FluentValidation;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Orders.Models;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;
using Restaurant.Shared.Extensions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record CreateOrderCommand(
    long TableId,
    long WaiterId,
    List<CreateOrderItemDto> Items
) : ICommand<OrderDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("Table id must be greater than 0");

        RuleFor(x => x.WaiterId)
            .GreaterThan(0)
            .WithMessage("Waiter id must be greater than 0");
    }
}

internal sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly TimeProvider _timeProvider;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        TimeProvider timeProvider)
    {
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            TableId = request.TableId,
            WaiterId = request.WaiterId,
            Status = OrderStatus.Created,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
        };

        decimal total = 0;

        foreach (var item in request.Items)
        {
            var dish = await _dishRepository.GetByIdAsync(item.DishId, cancellationToken);

            if (dish is null)
                throw new BusinessLogicException(DishErrors.NotFound);

            var orderItem = new OrderItem
            {
                DishId = dish.Id,
                Quantity = item.Quantity,
                Price = dish.Price,
                Status = OrderItemStatus.Pending
            };

            total += dish.Price * item.Quantity;

            var dishInventory = await _inventoryRepository.GetByIdAsync(item.DishId, cancellationToken);

            if (dishInventory is null)
                throw new BusinessLogicException(DishErrors.NotFound);

            if (dishInventory.Quantity < 1)
                throw new ResourceNotFoundException(InventoryErrors.OutOfStock);

            order.OrderItems.Add(orderItem);
        }

        order.TotalPrice = total;

        await _orderRepository.AddAsync(order, cancellationToken);

        return new OrderDto
        {
            Id = order.Id,
            TableId = order.TableId,
            WaiterId = order.WaiterId,
            TotalPrice = order.TotalPrice,
            Status = order.Status.ToString(),
            Items = order.OrderItems.Select(x => new OrderItemDto
            {
                Id = x.Id,
                DishId = x.DishId,
                Quantity = x.Quantity,
                Price = x.Price,
                Status = x.Status.ToString()
            }).ToList()
        };
    }
}