using Restaurant.Application.Features.Carts.Models;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.Carts.Commands;

public sealed record UpdateCartCommand(long CartId,CreateCartCommand ) : ICommand<CartDto>;