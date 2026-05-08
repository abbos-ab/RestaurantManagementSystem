using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;
using Restaurant.Shared.Extensions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(
    long OrderId,
    OrderStatus Status
) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");
    }
}

internal sealed class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly TimeProvider _timeProvider;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository, 
        TimeProvider timeProvider)
    {
        _orderRepository = orderRepository;
        _timeProvider = timeProvider;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        order.Status = request.Status;
        order.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _orderRepository.UpdateAsync(order, cancellationToken);

        return true;
    }
}