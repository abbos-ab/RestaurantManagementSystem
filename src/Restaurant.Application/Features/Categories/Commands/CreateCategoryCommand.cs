using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Restaurant.Application.Features.Categories.Models;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Application.Features.Categories.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Categories.Commands;

public sealed record CreateCategoryCommand(
    string Name,
    string Description
) : ICommand<CategoryDto>;

// ReSharper disable once UnusedType.Global
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}

internal sealed class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryMapper _mapper;
    private readonly TimeProvider _timeProvider;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        CategoryMapper mapper,
        TimeProvider timeProvider)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _timeProvider = timeProvider;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var spec = new CategoryByNameSpec(request.Name);
        var exists = await _categoryRepository.AnyAsync(spec, cancellationToken);

        if (exists)
            throw new BusinessLogicException(CategoryErrors.AlreadyExists);
        
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc()
        };

        await _categoryRepository.AddAsync(category, cancellationToken);

        return _mapper.Map(category);
    }
}