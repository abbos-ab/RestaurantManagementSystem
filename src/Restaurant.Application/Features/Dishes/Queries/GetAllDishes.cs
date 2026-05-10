using Ardalis.Specification;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Dishes.Queries;

public sealed record GetAllDishes(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<DishDto>>;

internal sealed class GetAllDishesHandler : IQueryHandler<GetAllDishes, PaginatedResult<DishDto>>
{
    private readonly IDishRepository _dishRepository;
    private readonly DishMapper _mapper;

    public GetAllDishesHandler(IDishRepository dishRepository, DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<DishDto>> Handle(GetAllDishes request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Dish>();

        spec.Query
            .Include(x => x.Category)
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.Id);

        var dishes = await _dishRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _dishRepository.CountAsync(spec, cancellationToken);

        var mapperDish = _mapper.Map(dishes);

        return new PaginatedResult<DishDto>(mapperDish, totalCount);
    }
}