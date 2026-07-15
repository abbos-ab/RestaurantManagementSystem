using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Restaurant.Application.Features.Medias.Models;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Medias.Queries;

public sealed record GetMediaById(long MediaId) : IQuery<DishMediaDto>;

// ReSharper disable once UnusedType.Global
public sealed class GetPhotoByIdValidator : AbstractValidator<GetMediaById>
{
    public GetPhotoByIdValidator()
    {
        RuleFor(x => x.MediaId)
            .GreaterThan(0)
            .WithMessage("PhotoId most be greaten then 0");
    }
}

internal sealed class GetPhotoByIdHandler : IQueryHandler<GetMediaById, DishMediaDto>
{
    private readonly IDishMediaRepository _dishMediaRepository;
    private readonly DishMediaMapper _dishMediaMapper;
    private readonly IMemoryCache _memoryCache;

    public GetPhotoByIdHandler(IDishMediaRepository dishMediaRepository, DishMediaMapper dishMediaMapper,
        IMemoryCache memoryCache)
    {
        _dishMediaRepository = dishMediaRepository;
        _dishMediaMapper = dishMediaMapper;
        _memoryCache = memoryCache;
    }

    public async Task<DishMediaDto> Handle(GetMediaById request, CancellationToken cancellationToken)
    {
        var cacheKey = $"dishMedia_{request.MediaId}";

        if (_memoryCache.TryGetValue(cacheKey, out DishMediaDto dishMediaDto))
            return dishMediaDto;

        var dishMedia = await _dishMediaRepository.GetByIdAsync(request.MediaId, cancellationToken);
        if (dishMedia is null)
            throw new ResourceNotFoundException(MediaErrors.NotFound);

        dishMediaDto = _dishMediaMapper.Map(dishMedia);

        _memoryCache.Set(cacheKey, dishMediaDto, TimeSpan.FromDays(1));

        return dishMediaDto;
    }
}