using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Dishes.Commands;

public sealed record UpdateDishPriceCommand(long DishId, decimal Price) : ICommand<DishDto>;

// ReSharper disable once UnusedType.Global
public sealed class UpdateDishPriceCommandValidator : AbstractValidator<UpdateDishPriceCommand>
{
    public UpdateDishPriceCommandValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("DishId must be greater than 0");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price must not be empty");
    }
}

internal sealed class UpdateDishPriceCommandHandler : ICommandHandler<UpdateDishPriceCommand, DishDto>
{
    private readonly IDishRepository _dishRepository;
    private readonly DishMapper _dishMapper;

    public UpdateDishPriceCommandHandler(IDishRepository dishRepository, DishMapper dishMapper)
    {
        _dishRepository = dishRepository;
        _dishMapper = dishMapper;
    }

    public async Task<DishDto> Handle(UpdateDishPriceCommand request, CancellationToken cancellationToken)
    {
        var dish = await _dishRepository.GetByIdAsync(request.DishId, cancellationToken);
        if (dish is null)
            throw new BusinessLogicException(DishErrors.NotFound);

        dish.Price = request.Price;

        await _dishRepository.SaveChangesAsync(cancellationToken);

        return _dishMapper.Map(dish);
    }
}