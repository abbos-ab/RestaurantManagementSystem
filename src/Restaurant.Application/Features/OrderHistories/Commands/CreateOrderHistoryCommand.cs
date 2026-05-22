using FluentValidation;
using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.OrderHistories.Commands;

public sealed record CreateOrderHistoryCommand(
    long OrderId,
    OrderHistoryAction Action,
    string Description,
    long? UserId,
    long? OrderItemId
) : ICommand<OrderHistoryDto>;

// ReSharper disable once UnusedType.Global
public sealed class CreateOrderHistoryCommandValidator : AbstractValidator<CreateOrderHistoryCommand>
{
    public CreateOrderHistoryCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order Id must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty();
    }
}

internal sealed class CreateOrderHistoryCommandHandler : ICommandHandler<CreateOrderHistoryCommand, OrderHistoryDto>
{
    private readonly IOrderHistoryRepository _historyRepository;
    private readonly OrderHistoryMapper _historyMapper;
    private readonly TimeProvider _timeProvider;

    public CreateOrderHistoryCommandHandler(
        IOrderHistoryRepository historyRepository,
        OrderHistoryMapper historyMapper,
        TimeProvider timeProvider)
    {
        _historyRepository = historyRepository;
        _historyMapper = historyMapper;
        _timeProvider = timeProvider;
    }

    public async Task<OrderHistoryDto> Handle(CreateOrderHistoryCommand request, CancellationToken cancellationToken)
    {
        var orderHistory = new OrderHistory
        {
            OrderId = request.OrderId,
            Action = request.Action,
            Description = request.Description,
            UserId = request.UserId,
            OrderItemId = request.OrderItemId,
            CreatedAt = _timeProvider.GetUtcNow().UtcDateTime
        };

        await _historyRepository.AddAsync(orderHistory, cancellationToken);

        return _historyMapper.Map(orderHistory);
    }
}