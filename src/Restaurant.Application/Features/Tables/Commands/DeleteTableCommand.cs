using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Tables.Commands;

public sealed record DeleteTableCommand(long TableId) : ICommand<bool>;

internal sealed class DeleteTableCommandHandler : ICommandHandler<DeleteTableCommand, bool>
{
    private readonly ITableRepository _tableRepository;

    public DeleteTableCommandHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<bool> Handle(DeleteTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (table is null)
            throw new BusinessLogicException(TableErrors.NotFound);

        await _tableRepository.DeleteAsync(table, cancellationToken);
        return true;
    }
}