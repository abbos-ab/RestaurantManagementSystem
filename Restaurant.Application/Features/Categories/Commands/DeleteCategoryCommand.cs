using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Shared.CQRS.Commands;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Categories.Commands;

public sealed record DeleteCategoryCommand(long Id) : ICommand<bool>;

internal sealed class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            throw new BusinessLogicException(CategoryErrors.NotFound);

        await _categoryRepository.DeleteAsync(category, cancellationToken);

        return true;
    }
}