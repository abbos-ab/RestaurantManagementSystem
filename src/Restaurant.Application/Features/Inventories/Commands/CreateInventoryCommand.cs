using FluentValidation;
using Restaurant.Application.Features.Dishes;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Inventories.Models;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Application.Features.Inventories.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Extensions;

namespace Restaurant.Application.Features.Inventories.Commands;

public sealed record CreateInventoryCommand(
    long DishId,
    int Quantity
) : ICommand<InventoryDto>;

// ReSharper disable once UnusedType.Global
internal class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0);
    }
}

internal sealed class CreateInventoryCommandHandler 
    : ICommandHandler<CreateInventoryCommand, InventoryDto>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IDishRepository _dishRepository;
    private readonly TimeProvider _timeProvider;

    public CreateInventoryCommandHandler(
        IInventoryRepository inventoryRepository,
        IDishRepository dishRepository, 
        TimeProvider timeProvider)
    {
        _inventoryRepository = inventoryRepository;
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
    }

    public async Task<InventoryDto> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var spec = new InventoryByDishIdSpec(request.DishId);
        var exists =  await _inventoryRepository.AnyAsync(spec, cancellationToken);

        if (exists)
            throw new BusinessLogicException(InventoryErrors.AlreadyExists);
        
        var dish = await _dishRepository.GetByIdAsync(request.DishId, cancellationToken);

        if (dish is null)
            throw new BusinessLogicException(DishErrors.NotFound);

        var inventory = new Inventory
        {
            DishId = request.DishId,
            Quantity = request.Quantity,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
        };

        await _inventoryRepository.AddAsync(inventory, cancellationToken);

        return new InventoryDto
        {
            Id = inventory.Id,
            DishId = inventory.DishId,
            DishName = dish.Name,
            Quantity = inventory.Quantity
        };
    }
}