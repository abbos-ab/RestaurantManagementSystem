using FluentValidation;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Categories.Queries;

public sealed record GetCategoryById(long Id) : IQuery<CategoryDto?>;

// ReSharper disable once UnusedType.Global
public class GetCategoryByIdValidator : AbstractValidator<GetCategoryById>
{
    public GetCategoryByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}

internal sealed class GetCategoryByIdHandler : IQueryHandler<GetCategoryById, CategoryDto?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _mapper;

    public GetCategoryByIdHandler(
        ICategoryRepository categoryRepository,
        CategoryMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(GetCategoryById request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            return null;

        return _mapper.Map(category);
    }
}
