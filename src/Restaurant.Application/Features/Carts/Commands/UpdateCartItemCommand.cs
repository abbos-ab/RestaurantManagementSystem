using FluentValidation;
using Restaurant.Application.Features.Carts.Models;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Application.Features.Carts.Specifications;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Inventories.Specifications;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Carts.Commands;

public sealed record UpdateCartItemCommand(long CartId, CreateCartItemDto Item) : ICommand<CartItemDto>;

public sealed class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0)
            .WithMessage("Cart id must be greater than 0");

        RuleFor(x => x.Item)
            .SetValidator(new CreateCartItemDtoValidator());
    }
}

internal sealed class UpdateCartItemCommandHandler : ICommandHandler<UpdateCartItemCommand, CartItemDto>
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IDishRepository _dishRepository;
    private readonly TimeProvider _timeProvider;
    private readonly CartMapper _mapper;

    public UpdateCartItemCommandHandler(
        ICartItemRepository cartItemRepository,
        ICartRepository cartRepository,
        IInventoryRepository inventoryRepository,
        IDishRepository dishRepository,
        TimeProvider timeProvider,
        CartMapper mapper)
    {
        _cartItemRepository = cartItemRepository;
        _cartRepository = cartRepository;
        _inventoryRepository = inventoryRepository;
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
    }

    public async Task<CartItemDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);

        if (cart is null)
            throw new BusinessLogicException(CartErrors.NotFound);

        var cartItemSpec = new CartItemByDishIdSpec(request.CartId, request.Item.DishId);
        var cartItem = await _cartItemRepository.FirstOrDefaultAsync(cartItemSpec, cancellationToken);

        if (cartItem is null)
            throw new BusinessLogicException(CartItemErrors.NotFound);

        var dish = await _dishRepository.GetByIdAsync(request.Item.DishId, cancellationToken);
        if (dish is null)
            throw new BusinessLogicException(DishErrors.NotFound);

        var inventorySpec = new InventoryByDishIdSpec(request.Item.DishId);
        var dishInventory = await _inventoryRepository.FirstOrDefaultAsync(inventorySpec, cancellationToken);

        if (dishInventory is null)
            throw new BusinessLogicException(InventoryErrors.NotFound);

        if (dishInventory.Quantity < request.Item.Quantity)
            throw new BusinessLogicException(InventoryErrors.OutOfStock);

        cartItem.DishId = request.Item.DishId;
        cartItem.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();
        cartItem.Quantity = request.Item.Quantity;
        cartItem.Price = dish.Price * request.Item.Quantity;

        await _cartItemRepository.UpdateAsync(cartItem, cancellationToken);
        await _cartItemRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map(cartItem);
    }
}