using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Waitors.Commands;

public sealed record TakeOrderCommand(long WaiterId, long OrderId ) : ICommand<OrderDto>;

// ReSharper disable once UnusedType.Global
public sealed class TakeOrderCommandValidator : AbstractValidator<TakeOrderCommand>
{
    public TakeOrderCommandValidator()
    {
        RuleFor(x => x.WaiterId)
            .GreaterThan(0)
            .WithMessage("WaiterId must be greater than 0");
        
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");
    }
}

internal sealed class TakeOrderCommandHandler : ICommandHandler<TakeOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public TakeOrderCommandHandler(IOrderRepository orderRepository, IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<OrderDto> Handle(TakeOrderCommand request, CancellationToken cancellationToken)
    {
        var waiter = await _userRepository.GetByIdAsync(request.WaiterId, cancellationToken);
        if (waiter is null)
            throw new BusinessLogicException();
    }
}