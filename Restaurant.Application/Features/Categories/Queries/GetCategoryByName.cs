using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Shared.Persistence;

namespace Restaurant.Application.Features.Categories.Queries;

public sealed record GetCategoryByName(string Name) : IQuery<List<CategoryDto>>;

public sealed class GetCategoryByNameValidator : AbstractValidator<GetCategoryByName>
{
    public GetCategoryByNameValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}

internal sealed class GetCategoryByNameHandler : IQueryHandler<GetCategoryByName, List<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _mapper;

    public GetCategoryByNameHandler(
        ICategoryRepository categoryRepository,
        CategoryMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoryByName request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Category>();
        spec.Query.Where(x => x.Name.Contains(request.Name));

        var categories = await _categoryRepository.ListAsync(spec, cancellationToken);
        
        return categories.Select(x => _mapper.Map(x)).ToList();
    }
}