using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Application.Features.Dishes.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;
using Restaurant.Shared.Extensions;

namespace Restaurant.Application.Features.Dishes.Commands;

public sealed record CreateDishCommand(
    string Name,
    long CategoryId,
    decimal Price,
    string Description,
    bool IsActive
) : ICommand<DishDto>;

// ReSharper disable once UnusedType.Global
internal class CreateDishCommandValidator : AbstractValidator<CreateDishCommand>
{
    public CreateDishCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}

internal sealed class CreateDishCommandHandler : ICommandHandler<CreateDishCommand, DishDto>
{
    private readonly IDishRepository _dishRepository;
    private readonly TimeProvider _timeProvider;
    private readonly DishMapper _mapper;

    public CreateDishCommandHandler(
        IDishRepository dishRepository,
        TimeProvider timeProvider,
        DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
    }

    public async Task<DishDto> Handle(CreateDishCommand request, CancellationToken cancellationToken)
    {
        var spec = new DishByNameSpec(request.Name);
        var exists = await _dishRepository.AnyAsync(spec, cancellationToken);
        
        if (exists)
            throw new BusinessLogicException(DishErrors.AlreadyExists);
                
        var dish = new Dish
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            Price = request.Price,
            Description = request.Description,
            IsActive = request.IsActive,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
        };

        await _dishRepository.AddAsync(dish, cancellationToken);

        return _mapper.Map(dish);
    }
}