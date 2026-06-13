using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Dishes.Queries;

public sealed record GetDishesByCategory(long CategoryId, PaginationInfo PaginationInfo)
    : IQuery<PaginatedResult<DishDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetDishesByCategoryValidator : AbstractValidator<GetDishesByCategory>
{
    public GetDishesByCategoryValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("CategoryId must be greater than 0");
    }
}

internal sealed class GetDishesByCategoryHandler : IQueryHandler<GetDishesByCategory, PaginatedResult<DishDto>>
{
    private readonly IDishRepository _dishRepository;
    private readonly DishMapper _mapper;

    public GetDishesByCategoryHandler(IDishRepository dishRepository, DishMapper mapper)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<DishDto>> Handle(GetDishesByCategory request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Dish>();
        spec.Query.Where(x => x.CategoryId == request.CategoryId);
        
        var dishes = await _dishRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _dishRepository.CountAsync(spec, cancellationToken);

        var mappedDish = _mapper.Map(dishes);

        return new PaginatedResult<DishDto>(mappedDish, totalCount);
    }
}