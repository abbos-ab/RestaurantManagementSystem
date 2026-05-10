using FluentValidation;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Extensions;

namespace Restaurant.Application.Features.Inventories.Commands;

public sealed record UpdateInventoryCommand(
    long Id,
    int Quantity
) : ICommand<InventoryDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateInventoryCommandValidator : AbstractValidator<UpdateInventoryCommand>
{
    public UpdateInventoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
        
        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity must be greater or equal 0");
    }
}

internal sealed class UpdateInventoryCommandHandler 
    : ICommandHandler<UpdateInventoryCommand, InventoryDto>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly TimeProvider _timeProvider;

    public UpdateInventoryCommandHandler(IInventoryRepository inventoryRepository, TimeProvider timeProvider)
    {
        _inventoryRepository = inventoryRepository;
        _timeProvider = timeProvider;
    }

    public async Task<InventoryDto> Handle(UpdateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (inventory is null)
            throw new BusinessLogicException(InventoryErrors.NotFound);

        inventory.Quantity = request.Quantity;
        inventory.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

        return new InventoryDto
        {
            Id = inventory.Id,
            DishId = inventory.DishId,
            Quantity = inventory.Quantity,
        };
    }
}