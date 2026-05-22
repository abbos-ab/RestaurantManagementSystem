using FluentValidation;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Dishes.Commands;

public sealed record DeleteDishCommand(long Id) : ICommand<bool>;

// ReSharper disable once UnusedType.Global
internal class DeleteDishCommandValidator : AbstractValidator<DeleteDishCommand>
{
    public DeleteDishCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}

internal sealed class DeleteDishCommandHandler : ICommandHandler<DeleteDishCommand, bool>
{
    private readonly IDishRepository _dishRepository;

    public DeleteDishCommandHandler(IDishRepository dishRepository)
    {
        _dishRepository = dishRepository;
    }

    public async Task<bool> Handle(DeleteDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await _dishRepository.GetByIdAsync(request.Id, cancellationToken);

        if (dish is null)
            throw new BusinessLogicException(DishErrors.NotFound);

        dish.IsActive = false;
        await _dishRepository.UpdateAsync(dish, cancellationToken);

        return true;
    }
}