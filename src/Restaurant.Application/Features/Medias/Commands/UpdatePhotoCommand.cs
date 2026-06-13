using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Medias.Commands;

public sealed record UpdatePhotoCommand(
    long PhotoId,
    int Order,
    double FileWidth,
    double FileHeight
) : ICommand;

internal sealed class UpdatePhotoCommandHandler : ICommandHandler<UpdatePhotoCommand>
{
    private readonly IDishMediaRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePhotoCommandHandler(IDishMediaRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
    {
        var media = await _repository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (media is null)
            throw new ResourceNotFoundException(MediaErrors.NotFound);

        media.SortOrder = request.Order;
        media.FileWidth = request.FileWidth;
        media.FileHeight = request.FileHeight;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}