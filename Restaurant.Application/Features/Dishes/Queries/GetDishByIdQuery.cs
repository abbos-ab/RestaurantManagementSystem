using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Dishes.Queries;

public sealed record GetDishByIdQuery(long Id) : IQuery<DishDto?>;

// ReSharper disable once UnusedType.Global
internal class GetDishByIdQueryValidator : AbstractValidator<GetDishByIdQuery>
{
    public GetDishByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}

internal sealed class GetDishByIdQueryHandler : IQueryHandler<GetDishByIdQuery, DishDto?>
{
    private readonly IDishRepository _dishRepository;
    private readonly DishMapper _mapper;

    public GetDishByIdQueryHandler(IDishRepository dishRepository, DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<DishDto?> Handle(GetDishByIdQuery request, CancellationToken cancellationToken)
    {
        var dish = await _dishRepository.GetByIdAsync(request.Id, cancellationToken);

        if (dish is null)
            return null;

        return _mapper.Map(dish);
    }
}