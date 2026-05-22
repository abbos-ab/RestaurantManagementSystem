using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Categories.Commands;

public sealed record UpdateCategoryCommand(
    long Id,
    string Name,
    string Description
) : ICommand<CategoryDto>;

// ReSharper disable once UnusedType.Global
public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty();
    }
}

internal sealed class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly TimeProvider _timeProvider;
    private readonly CategoryMapper _mapper;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        TimeProvider timeProvider,
        CategoryMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _timeProvider = timeProvider;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new BusinessLogicException(CategoryErrors.NotFound);

        category.Name = request.Name;
        category.Description = request.Description;
        category.UpdatedAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        await _categoryRepository.UpdateAsync(category, cancellationToken);

        return _mapper.Map(category);
    }
}