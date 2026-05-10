using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record DeleteOrderCommand(long TableId, long OrderId) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");

        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");
    }
}

internal sealed class DeleteOrderCommandHandler : ICommandHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var orderByTableIdAndOrderIdSpec = new OrderByTableIdAndOrderId(request.TableId, request.OrderId);
        var order = await _orderRepository.FirstOrDefaultAsync(orderByTableIdAndOrderIdSpec, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        if (order.Status == OrderStatus.Completed)
            throw new BusinessLogicException(OrderErrors.CannotDeleteCompletedOrder);

        await _orderRepository.DeleteAsync(order, cancellationToken);
        return true;
    }
}