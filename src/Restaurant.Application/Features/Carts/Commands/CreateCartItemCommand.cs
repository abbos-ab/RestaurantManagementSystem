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
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Carts.Commands;

public sealed record CreateCartItemCommand(long CartId, List<CreateCartItemDto> Items) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class CreateCartItemCommandValidator : AbstractValidator<CreateCartItemCommand>
{
    public CreateCartItemCommandValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0)
            .WithMessage("Cart Id must be greater than 0");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateCartItemDtoValidator());
    }
}

internal sealed class CreateCartItemCommandHandler : ICommandHandler<CreateCartItemCommand, bool>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IDishRepository _dishRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public CreateCartItemCommandHandler(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IDishRepository dishRepository,
        IInventoryRepository inventoryRepository)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _dishRepository = dishRepository;
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(CreateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);

        if (cart is null)
            throw new BusinessLogicException(CartErrors.NotFound);

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

            var cartItemByDishIdSpec = new CartItemByDishIdSpec(request.CartId, item.DishId);
            var cartItem = await _cartItemRepository.FirstOrDefaultAsync(cartItemByDishIdSpec, cancellationToken);

            if (cartItem is null)
            {
                var newCartItem = new CartItem
                {
                    CartId = request.CartId,
                    DishId = item.DishId,
                    Quantity = item.Quantity,
                    Price = dish.Price * item.Quantity,
                };

                total += dish.Price * item.Quantity;

                await _cartItemRepository.AddAsync(newCartItem, cancellationToken);
            }
            else
            {
                cartItem.Quantity += item.Quantity;
                cartItem.Price += dish.Price * item.Quantity;

                total += dish.Price * item.Quantity;

                await _cartItemRepository.UpdateAsync(cartItem, cancellationToken);
            }
        }

        cart.TotalPrice += total;

        await _cartItemRepository.SaveChangesAsync(cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}