using FluentValidation;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Application.Features.Carts.Specifications;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Carts.Commands;

public sealed record DeleteCartCommand(long CartId, long TableId) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteCartCommandValidator : AbstractValidator<DeleteCartCommand>
{
    public DeleteCartCommandValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0)
            .WithMessage("CartId must be greater than 0");

        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");
    }
}

internal sealed class DeleteCartCommandHandler : ICommandHandler<DeleteCartCommand, bool>
{
    private readonly ICartRepository _cartRepository;

    public DeleteCartCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<bool> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        var spec = new CartByTableIdAndCartIdSpec(request.TableId, request.CartId);
        var cart = await _cartRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (cart is null)
            throw new BusinessLogicException(CartErrors.NotFound);

        await _cartRepository.DeleteAsync(cart, cancellationToken);
        await _cartRepository.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}