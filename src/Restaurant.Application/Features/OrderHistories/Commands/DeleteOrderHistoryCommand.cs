using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.OrderHistories.Commands;

public sealed record DeleteOrderHistoryCommand(long Id) : ICommand;

internal sealed class DeleteOrderHistoryCommandHandler
    : ICommandHandler<DeleteOrderHistoryCommand>
{
    private readonly IOrderHistoryRepository _historyRepository;

    public DeleteOrderHistoryCommandHandler(IOrderHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }

    public async Task Handle(DeleteOrderHistoryCommand request, CancellationToken cancellationToken)
    {
        var orderHistory = await _historyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (orderHistory is null)
            throw new BusinessLogicException(OrderHistoryErrors.NotFound);

        await _historyRepository.DeleteAsync(orderHistory, cancellationToken);
    }
}