using FluentValidation;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Inventories.Commands;

public sealed record DeleteInventoryCommand(long Id) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
public sealed class DeleteInventoryCommandValidator : AbstractValidator<DeleteInventoryCommand>
{
    public DeleteInventoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id cannot be less than zero");
    }
}

internal sealed class DeleteInventoryCommandHandler : ICommandHandler<DeleteInventoryCommand, bool>
{
    private readonly IInventoryRepository _inventoryRepository;

    public DeleteInventoryCommandHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<bool> Handle(DeleteInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (inventory is null)
            throw new BusinessLogicException(InventoryErrors.NotFound);

        await _inventoryRepository.DeleteAsync(inventory, cancellationToken);

        return true;
    }
}