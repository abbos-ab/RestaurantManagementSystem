using FluentValidation;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Application.Features.Carts.Specifications;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Carts.Commands;

public sealed record DeleteCartItemsCommand(long CartId, List<long> DishIds) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteCartItemsCommandValidator : AbstractValidator<DeleteCartItemsCommand>
{
    public DeleteCartItemsCommandValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0)
            .WithMessage("CartId must be greater than 0");

        RuleFor(x => x.DishIds).NotEmpty();
    }
}

internal sealed class DeleteCartItemsCommandHandler : ICommandHandler<DeleteCartItemsCommand, bool>
{
    private readonly ICartItemRepository _cartItemRepository;
    private readonly ICartRepository _cartRepository;

    private DeleteCartItemsCommandHandler(ICartItemRepository cartItemRepository, ICartRepository cartRepository)
    {
        _cartItemRepository = cartItemRepository;
        _cartRepository = cartRepository;
    }

    public async Task<bool> Handle(DeleteCartItemsCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        
        if (cart is null)
            throw new BusinessLogicException(CartErrors.NotFound);

        foreach (var item in request.DishIds)
        {
            var spec = new CartItemByDishIdSpec(request.CartId, item);
            var cartItem = await _cartItemRepository.FirstOrDefaultAsync(spec, cancellationToken);

            if (cartItem is null)
                throw new BusinessLogicException(CartItemErrors.NotFound);

            await _cartItemRepository.DeleteAsync(cartItem, cancellationToken);
        }

        await _cartItemRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}