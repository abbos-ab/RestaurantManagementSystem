using Ardalis.Specification;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Medias.Commands;

public record DeletePhotoCommand(long MediaId) : ICommand;

// ReSharper disable once UnusedType.Global
internal sealed class DeletePhotoCommandValidation : AbstractValidator<DeletePhotoCommand>
{
    public DeletePhotoCommandValidation()
    {
        RuleFor(x => x.MediaId)
            .GreaterThan(0)
            .WithMessage("MediaId must be greater than 0");
    }
}

internal sealed class DeletePhotoCommandHandler : ICommandHandler<DeletePhotoCommand>
{
    private readonly IDishMediaRepository _dishMediaRepository;
    private readonly IDishMediaRelationRepository _dishMediaRelationRepository;
    private readonly IMinioClientService _minioClientService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePhotoCommandHandler> _logger;

    public DeletePhotoCommandHandler(
        IDishMediaRepository dishMediaRepository,
        IDishMediaRelationRepository dishMediaRelationRepository,
        ILogger<DeletePhotoCommandHandler> logger,
        IUnitOfWork unitOfWork, 
        IMinioClientService minioClientService)
    {
        _dishMediaRepository = dishMediaRepository;
        _dishMediaRelationRepository = dishMediaRelationRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _minioClientService = minioClientService;
    }

    public async Task Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
    {
        var mediaSpec = new DbSpecification<DishMedia>();
        mediaSpec.Query.Where(x => x.Id == request.MediaId);

        var media = await _dishMediaRepository.FirstOrDefaultAsync(mediaSpec, cancellationToken);
        if (media is null)
            throw new ResourceNotFoundException(MediaErrors.NotFound);

        var mediaRelationSpec = new DbSpecification<DishMediaRelation>();
        mediaRelationSpec.Query.Where(x => x.MediaId == request.MediaId);

        var mediaRelation =
            await _dishMediaRelationRepository.FirstOrDefaultAsync(mediaRelationSpec, cancellationToken);
        if (mediaRelation is null)
            throw new ResourceNotFoundException(new Error("MediaRelation.NotFound", "MediaRelations not found"));

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _minioClientService.RemoveObjectAsync(media.Path);
            
            await _dishMediaRelationRepository.DeleteAsync(mediaRelation, cancellationToken);
            await _dishMediaRepository.DeleteAsync(media, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Deleted Dish Media with Id {MediaId}",
                media.Id
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Dish Media with Id {MediaId}",
                request.MediaId
            );

            await transaction.RollbackAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Failed to delete Dish Media with Id {request.MediaId}",
                ex);
        }
    }
}