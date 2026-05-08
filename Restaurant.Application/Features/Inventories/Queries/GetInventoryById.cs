using FluentValidation;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Shared.CQRS.Queries;

namespace Restaurant.Application.Features.Inventories.Queries;

public sealed record GetInventoryById(long Id) : IQuery<InventoryDto?>;

// ReSharper disable once UnusedType.Global
public sealed class GetInventoryByIdValidator : AbstractValidator<GetInventoryById>
{
    public GetInventoryByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id cannot be less than zero");
    }
}

internal sealed class GetInventoryByIdHandler : IQueryHandler<GetInventoryById, InventoryDto?>
{
    private readonly IInventoryRepository _inventoryRepository;

    public GetInventoryByIdHandler(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<InventoryDto?> Handle(GetInventoryById request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (inventory is null)
            return null;

        return new InventoryDto
        {
            Id = inventory.Id,
            DishId = inventory.DishId,
            Quantity = inventory.Quantity
        };
    }
}