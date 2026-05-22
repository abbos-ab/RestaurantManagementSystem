using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Waiters.Commands;

public sealed record CancelOrderCommand(long OrderId, long WaiterId) : ICommand<bool>;

public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");

        RuleFor(x => x.WaiterId)
            .GreaterThan(0)
            .WithMessage("WaiterId must be greater than 0");
    }
}

internal sealed class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var waiter = await _userRepository.GetByIdAsync(request.WaiterId, cancellationToken);
        if (waiter is null || waiter.IsActive == false)
            throw new BusinessLogicException(WaiterErrors.NotFound);

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new BusinessLogicException(WaiterErrors.OrderNotFound);

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Rejected)
        {
            switch (order.Status)
            {
                case OrderStatus.Completed:
                    throw new BusinessLogicException(WaiterErrors.OrderCompleted);

                case OrderStatus.Rejected:
                    throw new BusinessLogicException(WaiterErrors.OrderRejected);
            }
        }

        if (order.WaiterId != request.WaiterId)
            throw new BusinessLogicException(WaiterErrors.AlreadyTaken);

        order.WaiterId = null;

        await _orderRepository.UpdateAsync(order, cancellationToken);
        return true;
    }
}