using FluentValidation;
using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.OrderHistories.Commands;

public sealed record UpdateOrderHistoryCommand(
    long Id,
    OrderHistoryAction Action,
    string Description
) : ICommand<OrderHistoryDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateOrderHistoryCommandValidator
    : AbstractValidator<UpdateOrderHistoryCommand>
{
    public UpdateOrderHistoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage($"Order history id must be greater than 0");

        RuleFor(x => x.Description)
            .NotEmpty();
    }
}

internal sealed class UpdateOrderHistoryCommandHandler : ICommandHandler<UpdateOrderHistoryCommand, OrderHistoryDto>
{
    private readonly IOrderHistoryRepository _historyRepository;
    private readonly OrderHistoryMapper _historyMapper;

    public UpdateOrderHistoryCommandHandler(
        IOrderHistoryRepository historyRepository,
        OrderHistoryMapper historyMapper)
    {
        _historyRepository = historyRepository;
        _historyMapper = historyMapper;
    }

    public async Task<OrderHistoryDto> Handle(UpdateOrderHistoryCommand request, CancellationToken cancellationToken)
    {
        var orderHistory = await _historyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (orderHistory is null)
            throw new BusinessLogicException(OrderHistoryErrors.NotFound);

        orderHistory.Action = request.Action;
        orderHistory.Description = request.Description;

        await _historyRepository.SaveChangesAsync(cancellationToken);

        return _historyMapper.Map(orderHistory);
    }
}