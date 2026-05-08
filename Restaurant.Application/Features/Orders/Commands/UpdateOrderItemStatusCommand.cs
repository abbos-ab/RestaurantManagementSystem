using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;
using Restaurant.Shared.Extensions;

namespace Restaurant.Application.Features.Orders.Commands;

public sealed record UpdateOrderItemStatusCommand(
    long OrderItemId,
    OrderItemStatus Status
) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public class UpdateOrderItemStatusCommandValidator : AbstractValidator<UpdateOrderItemStatusCommand>
{
    public UpdateOrderItemStatusCommandValidator()
    {
        RuleFor(x => x.OrderItemId)
            .GreaterThan(0)
            .WithMessage("OrderItemId must be greater than 0");
    }
}

internal sealed class UpdateOrderItemStatusCommandHandler : ICommandHandler<UpdateOrderItemStatusCommand, bool>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly TimeProvider _timeProvider;

    public UpdateOrderItemStatusCommandHandler(
        IOrderItemRepository orderItemRepository,
        TimeProvider timeProvider)
    {
        _orderItemRepository = orderItemRepository;
        _timeProvider = timeProvider;
    }

    public async Task<bool> Handle(UpdateOrderItemStatusCommand request, CancellationToken cancellationToken)
    {
        var item = await _orderItemRepository.GetByIdAsync(request.OrderItemId, cancellationToken);

        if (item is null)
            throw new BusinessLogicException(OrderItemErrors.NotFound);

        item.Status = request.Status;
        item.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        if (request.Status == OrderItemStatus.Ready)
            item.PreparedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        if (request.Status == OrderItemStatus.Served)
            item.ServedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _orderItemRepository.UpdateAsync(item, cancellationToken);

        return true;
    }
}