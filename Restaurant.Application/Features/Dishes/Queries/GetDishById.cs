using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Dishes.Queries;

public sealed record GetDishById(long Id) : IQuery<DishDto?>;

// ReSharper disable once UnusedType.Global
public class GetDishByIdValidator : AbstractValidator<GetDishById>
{
    public GetDishByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}

internal sealed class GetDishByIdHandler : IQueryHandler<GetDishById, DishDto?>
{
    private readonly IDishRepository _dishRepository;
    private readonly DishMapper _mapper;

    public GetDishByIdHandler(IDishRepository dishRepository, DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<DishDto?> Handle(GetDishById request, CancellationToken cancellationToken)
    {
        var dish = await _dishRepository.GetByIdAsync(request.Id, cancellationToken);

        if (dish is null)
            throw new ResourceNotFoundException(DishErrors.NotFound);

        return _mapper.Map(dish);
    }
}