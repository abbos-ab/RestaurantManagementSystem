using FluentValidation;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Inventories.Specifications;
using Restaurant.Application.Features.Orders.Models;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Tables;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record CreateOrderCommand(
    long TableId,
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

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemDtoValidator());
    }
}

internal sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ITableRepository _tableRepository;
    private readonly TimeProvider _timeProvider;
    private readonly OrderMapper _mapper;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        TimeProvider timeProvider, OrderMapper mapper,
        ITableRepository tableRepository, 
        IOrderItemRepository orderItemRepository)
    {
        _orderRepository = orderRepository;
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
        _tableRepository = tableRepository;
        _orderItemRepository = orderItemRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);
        if (table is null)
            throw new BusinessLogicException(TableErrors.NotFound);

        if (table.Status is not TableStatus.Available)
        {
            switch (table.Status)
            {
                case TableStatus.Disabled:
                    throw new ResourceNotFoundException(TableErrors.TableDisabled);
                    break;

                case TableStatus.Occupied:
                    throw new ResourceNotFoundException(TableErrors.TableOccupied);
                    break;

                case TableStatus.Reserved:
                    throw new ResourceNotFoundException(TableErrors.TableReserved);
                    break;
            }
        }

        var order = new Order
        {
            TableId = request.TableId,
            Status = OrderStatus.Created,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        decimal total = 0;
        List<OrderItem> orderItems = new List<OrderItem>();
        foreach (var item in request.Items)
        {
            var dish = await _dishRepository.GetByIdAsync(item.DishId, cancellationToken);

            if (dish is null)
                throw new BusinessLogicException(DishErrors.NotFound);

            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                DishId = dish.Id,
                Quantity = item.Quantity,
                Price = dish.Price * item.Quantity,
                Status = OrderItemStatus.Pending
            };

            total += dish.Price * item.Quantity;

            var spec = new InventoryByDishIdSpec(item.DishId);
            var dishInventory = await _inventoryRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (dishInventory is null)
                throw new BusinessLogicException(InventoryErrors.NotFound);

            if (dishInventory.Quantity < item.Quantity)
                throw new ResourceNotFoundException(InventoryErrors.OutOfStock);

            orderItems.Add(orderItem);

            dishInventory.Quantity -= item.Quantity;
            await _inventoryRepository.SaveChangesAsync(cancellationToken);
        }

        await _orderItemRepository.AddRangeAsync(orderItems, cancellationToken);
        
        order.TotalPrice += total;
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map(order);
    }
}