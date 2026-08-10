using FluentValidation;
using MassTransit;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Waiters.Specifications;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Contracts.Events;

namespace Restaurant.Application.Features.Waiters.Commands;

public sealed record CallWaiterCommand(long TableId) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class CallWaiterCommandValidator : AbstractValidator<CallWaiterCommand>
{
    public CallWaiterCommandValidator()
    {
        RuleFor(x => x.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");
    }
}

internal sealed class CallWaiterCommandHandler : ICommandHandler<CallWaiterCommand, bool>
{
    private readonly IOrderRepository _orderRepository;

    public CallWaiterCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(CallWaiterCommand request, CancellationToken cancellationToken)
    {
        var spec = new WaiterByTableIdSpec(request.TableId);
        var waiter = await _orderRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (waiter is null)
            throw new ResourceNotFoundException(WaiterErrors.NotFound);

        return true;
    }
}