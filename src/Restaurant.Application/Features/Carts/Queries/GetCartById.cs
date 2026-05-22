using FluentValidation;
using Restaurant.Application.Features.Carts.Models;
using Restaurant.Application.Features.Carts.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Carts.Queries;

public sealed record GetCartById(long CartId) : IQuery<CartDto>;

// ReSharper disable once UnusedType.Global
public sealed class GetCartByIdValidator : AbstractValidator<GetCartById>
{
    public GetCartByIdValidator()
    {
        RuleFor(x => x.CartId)
            .GreaterThan(0)
            .WithMessage("Cart id must be greater than 0");
    }
}

internal sealed class GetCartByIdHandler : IQueryHandler<GetCartById, CartDto>
{
    private readonly ICartRepository _cartRepository;

    public GetCartByIdHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public Task<CartDto> Handle(GetCartById request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}