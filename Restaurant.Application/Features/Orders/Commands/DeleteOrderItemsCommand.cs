using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record DeleteOrderItemsCommand(long OrderId, List<long> OrderItems) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteOrderItemsCommandValidator : AbstractValidator<DeleteOrderItemsCommand>
{
    public DeleteOrderItemsCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order Id must be greater than 0");

        RuleForEach(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("Order Items must not be empty");
    }
}

internal class DeleteOrderItemsCommandHandler : ICommandHandler<DeleteOrderItemsCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;

    public DeleteOrderItemsCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
    }

    public async Task<bool> Handle(DeleteOrderItemsCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        foreach (var item in request.OrderItems)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(item, cancellationToken);

            if (orderItem is null)
                throw new BusinessLogicException(OrderItemErrors.NotFound);

            await _orderItemRepository.DeleteAsync(orderItem, cancellationToken);
        }

        await _orderItemRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}