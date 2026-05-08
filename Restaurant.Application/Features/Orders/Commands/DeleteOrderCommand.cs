using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Commands;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record DeleteOrderCommand(long Id) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("OrderItemId must be greater than 0");
    }
}

internal sealed class DeleteOrderCommandHandler 
    : ICommandHandler<DeleteOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
            throw new Exception("Order not found");
        
        if (order.Status == OrderStatus.Completed)
            throw new Exception("Completed order cannot be deleted");

        await _orderRepository.DeleteAsync(order, cancellationToken);

        return true;
    }
}