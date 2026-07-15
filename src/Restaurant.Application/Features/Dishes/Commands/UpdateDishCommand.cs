using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;

namespace Restaurant.Application.Features.Dishes.Commands;

public sealed record UpdateDishCommand(
    long Id,
    string? Name,
    long? CategoryId,
    string? Description
) : ICommand<DishDto>;

// ReSharper disable once UnusedType.Global
internal class UpdateDishCommandValidator : AbstractValidator<UpdateDishCommand>
{
    public UpdateDishCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);
    }
}

internal sealed class UpdateDishCommandHandler : ICommandHandler<UpdateDishCommand, DishDto>
{
    private readonly IDishRepository _dishRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DishMapper _mapper;

    public UpdateDishCommandHandler(
        IDishRepository dishRepository,
        TimeProvider timeProvider,
        DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
    }

    public async Task<DishDto> Handle(UpdateDishCommand request, CancellationToken cancellationToken)
    {
        var dish = await _dishRepository.GetByIdAsync(request.Id, cancellationToken);

        if (dish is null)
            throw new Exception("Dish not found");

        if (request.Name is not null)
            dish.Name = request.Name;

        if (request.CategoryId.HasValue)
            dish.CategoryId = request.CategoryId;

        if (request.Description is not null)
            dish.Description = request.Description;

        dish.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _dishRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map(dish);
    }
}