using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Dishes.Queries;

public record SearchDishQuery(
    string Text,
    PaginationInfo PaginationInfo
) : IQuery<PaginatedResult<DishDto>>;

// ReSharper disable once UnusedType.Global
public sealed class SearchDishQueryValidator : AbstractValidator<SearchDishQuery>
{
    public SearchDishQueryValidator()
    {
        RuleFor(q => q.Text)
            .NotEmpty()
            .WithMessage("Please specify a text");
    }
}

internal class SearchDishQueryHandler : IQueryHandler<SearchDishQuery, PaginatedResult<DishDto>>
{
    private readonly IDishRepository _dishRepository;
    private readonly DishMapper _mapper;

    public SearchDishQueryHandler(IDishRepository dishRepository, DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<DishDto>> Handle(SearchDishQuery request, CancellationToken cancellationToken)
    {
        var spec = new DbSpecification<Dish>();
        spec.Query.Where(x => x.Name.Contains(request.Text))
            .WithPagination(request.PaginationInfo);

        var dishes = await _dishRepository.ListAsync(spec, cancellationToken);
        var count = await _dishRepository.CountAsync(spec, cancellationToken);

        if (!dishes.Any())
            throw new ResourceNotFoundException(DishErrors.NotFound);

        var mappedDishes = _mapper.Map(dishes);

        return new PaginatedResult<DishDto>(mappedDishes, count);
    }
}