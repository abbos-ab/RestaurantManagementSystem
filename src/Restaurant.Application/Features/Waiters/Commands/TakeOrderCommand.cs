using FluentValidation;
using Restaurant.Application.Features.Orders;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Waiters.Commands;

public sealed record TakeOrderCommand(long OrderId, long WaiterId) : ICommand<OrderDto>;

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
    private readonly TimeProvider _timeProvider;
    private readonly OrderMapper _orderMapper;

    public TakeOrderCommandHandler(
        IOrderRepository orderRepository,
        IUserRepository userRepository,
        TimeProvider timeProvider, OrderMapper orderMapper)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _timeProvider = timeProvider;
        _orderMapper = orderMapper;
    }

    public async Task<OrderDto> Handle(TakeOrderCommand request, CancellationToken cancellationToken)
    {
        var waiter = await _userRepository.GetByIdAsync(request.WaiterId, cancellationToken);
        if (waiter is null)
            throw new BusinessLogicException(WaiterErrors.NotFound);

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
            throw new BusinessLogicException(WaiterErrors.OrderNotFound);

        if (order.WaiterId is not null && order.WaiterId != waiter.Id)
            throw new BusinessLogicException(WaiterErrors.AlreadyTaken);

        if (order.WaiterId == request.WaiterId)
            throw new BusinessLogicException(WaiterErrors.AlreadyAssigned);

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

        order.WaiterId = request.WaiterId;
        order.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _orderRepository.UpdateAsync(order, cancellationToken);
        return _orderMapper.Map(order);
    }
}