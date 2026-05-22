using FluentValidation;
using Restaurant.Application.Features.Carts.Models;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Application.Features.Carts.Specifications;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Inventories.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Carts.Commands;

public sealed record CreateCartCommand(
    long TableId,
    List<CreateCartItemDto> Items
) : ICommand<CartDto>;

// ReSharper disable once UnusedType.Global
public class CreateCartCommandValidator : AbstractValidator<CreateCartCommand>
{
    public CreateCartCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateCartItemDtoValidator());
    }
}

internal sealed class CreateCartCommandHandler : ICommandHandler<CreateCartCommand, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly CartMapper _cartMapper;

    public CreateCartCommandHandler(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        TimeProvider timeProvider,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository,
        CartMapper cartMapper)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _timeProvider = timeProvider;
        _dishRepository = dishRepository;
        _inventoryRepository = inventoryRepository;
        _cartMapper = cartMapper;
    }

    public async Task<CartDto> Handle(CreateCartCommand request, CancellationToken cancellationToken)
    {
        var cartByTableIdSepc = new CartByTableIdSpec(request.TableId);
        var existingCart = await _cartRepository.AnyAsync(cartByTableIdSepc, cancellationToken);

        if (existingCart)
            throw new BusinessLogicException(CartErrors.AlreadyExists);

        var cart = new Cart
        {
            TableId = request.TableId,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
        };

        await _cartRepository.AddAsync(cart, cancellationToken);

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

            var cartItem = new CartItem
            {
                CartId = cart.Id,
                DishId = item.DishId,
                Quantity = item.Quantity,
                Price = dish.Price * item.Quantity,
            };

            total += dish.Price * item.Quantity;

            await _cartItemRepository.AddAsync(cartItem, cancellationToken);
        }

        cart.TotalPrice += total;

        await _cartItemRepository.SaveChangesAsync(cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);
        return _cartMapper.Map(cart);
    }
}