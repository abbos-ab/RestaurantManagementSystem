using Ardalis.Specification;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Restaurant.Application.Features.Medias.Models;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Application.Features.Medias.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Medias.Queries;

public sealed record GetPhotoByDishId(long DishId) : IQuery<List<DishMediaDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetPhotoByDishIdValidator : AbstractValidator<GetPhotoByDishId>
{
    public GetPhotoByDishIdValidator()
    {
        RuleFor(x => x.DishId)
            .GreaterThan(0)
            .WithMessage("DishId must be greater than 0");
    }
}

internal sealed class GetPhotoByDishIdHandler : IQueryHandler<GetPhotoByDishId, List<DishMediaDto>>
{
    private readonly IDishMediaRelationRepository _dishMediaRelationRepository;
    private readonly IDishMediaRepository _mediaRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly DishMediaMapper _mapper;

    public GetPhotoByDishIdHandler(IDishMediaRelationRepository dishMediaRelationRepository,
        IDishMediaRepository mediaRepository, DishMediaMapper mapper, IMemoryCache memoryCache)
    {
        _dishMediaRelationRepository = dishMediaRelationRepository;
        _mediaRepository = mediaRepository;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }

    public async Task<List<DishMediaDto>> Handle(GetPhotoByDishId request, CancellationToken cancellationToken)
    {
        var cacheKey = $"dishMedia_{request.DishId}";

        if (_memoryCache.TryGetValue(cacheKey, out List<DishMediaDto> dishMediaDto))
            return dishMediaDto;

        var spec = new DishMediaIdsByDishIdSpec(request.DishId);
        var mediaIds = await _dishMediaRelationRepository.ListAsync(spec, cancellationToken);

        List<DishMedia> medias = new List<DishMedia>();
        DishMedia? media;
        foreach (var mediaId in mediaIds)
        {
            media = await _mediaRepository.GetByIdAsync(mediaId, cancellationToken);
            if (media is null)
                throw new ResourceNotFoundException(MediaErrors.NotFound);

            medias.Add(media);
        }

        dishMediaDto = _mapper.Map(medias);

        _memoryCache.Set(cacheKey, dishMediaDto, TimeSpan.FromDays(1));

        return dishMediaDto;
    }
}