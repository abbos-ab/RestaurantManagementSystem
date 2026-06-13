using FluentValidation;
using Restaurant.Application.Features.Notifications.Repositories;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Waiters.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

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
    private readonly INotificationRepository _notificationRepository;

    public CallWaiterCommandHandler(IOrderRepository orderRepository, INotificationRepository notificationRepository)
    {
        _orderRepository = orderRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<bool> Handle(CallWaiterCommand request, CancellationToken cancellationToken)
    {
        var spec = new WaiterByTableIdSpec(request.TableId);
        var waiter = await _orderRepository.FirstOrDefaultAsync(spec, cancellationToken);

        if (waiter == null)
            throw new BusinessLogicException(WaiterErrors.NotFound);

        Notification notification = new Notification
        {
            UserId = waiter.Value,
            Type = NotificationType.TableCalledWaiter,
        };

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}