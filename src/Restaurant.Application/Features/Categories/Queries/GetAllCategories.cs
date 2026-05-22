using Ardalis.Specification;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Categories.Queries;

public sealed record GetAllCategories(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<CategoryDto>>;

internal sealed class GetAllCategoriesHandler : IQueryHandler<GetAllCategories, PaginatedResult<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _mapper;

    public GetAllCategoriesHandler(
        ICategoryRepository categoryRepository,
        CategoryMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<CategoryDto>> Handle(GetAllCategories request,
        CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Category>();

        spec.Query
            .Include(x => x.Dishes)
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.Id);

        var categories = await _categoryRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _categoryRepository.CountAsync(spec, cancellationToken);

        var mapperCategory = _mapper.Map(categories);

        return new PaginatedResult<CategoryDto>(mapperCategory, totalCount);
    }
}