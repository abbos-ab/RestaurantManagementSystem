using FluentValidation;
using Microsoft.Extensions.Logging;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Application.Services;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.CQRS.Commands;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Medias.Commands;

public sealed record UploadPhotoCommand(
    long DishId,
    int Order,
    Stream Stream,
    long FileLength,
    string FileName,
    double FileWidth,
    double FileHeight,
    string ContentType
) : ICommand;

// ReSharper disable once UnusedType.Global
internal sealed class UploadPhotoCommandValidator : AbstractValidator<UploadPhotoCommand>
{
    public UploadPhotoCommandValidator()
    {
        RuleFor(x => x.Stream).NotEmpty();
    }
}

internal sealed class UploadPhotoCommandHandler : ICommandHandler<UploadPhotoCommand>
{
    private readonly IDishMediaRepository _dishMediaRepository;
    private readonly IDishMediaRelationRepository _dishMediaRelationRepository;
    private readonly IMinioClientService _minioClientService;
    private readonly ILogger<UploadPhotoCommandHandler> _logger;
    private readonly IImageCompressionService _imageCompressionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UploadPhotoCommandHandler(
        IDishMediaRepository dishMediaRepository,
        IDishMediaRelationRepository dishMediaRelationRepository,
        IUnitOfWork unitOfWork,
        IMinioClientService minioClientService,
        ILogger<UploadPhotoCommandHandler> logger,
        TimeProvider timeProvider,
        IImageCompressionService imageCompressionService)
    {
        _dishMediaRepository = dishMediaRepository;
        _dishMediaRelationRepository = dishMediaRelationRepository;
        _unitOfWork = unitOfWork;
        _minioClientService = minioClientService;
        _logger = logger;
        _timeProvider = timeProvider;
        _imageCompressionService = imageCompressionService;
    }

    public async Task Handle(UploadPhotoCommand request, CancellationToken cancellationToken)
    {
        var dishMedia = new DishMedia
        {
            CreatedAt = _timeProvider.GetLocalDateTimeNowKindUtc(),
            MediaType = MediaType.Image,
            Path = "",
            SortOrder = request.Order,
            FileSize = request.FileLength,
            FileHeight = request.FileHeight,
            FileWidth = request.FileWidth,
        };

        await using var transaction = await _unitOfWork.BeginTransactionAsync(CancellationToken.None);
        try
        {
            await _dishMediaRepository.AddAsync(dishMedia, CancellationToken.None);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            var (compressedStream, contentType, _) = await _imageCompressionService.CompressAsync(
                request.Stream,
                request.ContentType,
                cancellationToken
            );

            dishMedia.Path = DishMedia.GetMediaFullPath(
                dishId: dishMedia.Id,
                folder: Dish.DishMinioFolder,
                fileName: request.FileName
            );

            await _minioClientService.SaveObjectAsync(
                objectName: dishMedia.Path,
                stream: compressedStream,
                contentType: contentType
            );

            await _dishMediaRelationRepository.AddAsync(
                new DishMediaRelation
                {
                    DishId = request.DishId,
                    MediaId = dishMedia.Id,
                },
                CancellationToken.None
            );

            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            await transaction.CommitAsync(CancellationToken.None);

            _logger.LogInformation(
                "Photo uploaded successfully for dish {DishId}. Photo ID: {DishMediaId}",
                request.DishId,
                dishMedia.Id
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the photo");

            await transaction.RollbackAsync(CancellationToken.None);

            await _minioClientService.RemoveObjectAsync(dishMedia.Path);

            throw new InvalidOperationException("An error occurred while saving the photo", ex);
        }   
    }
}