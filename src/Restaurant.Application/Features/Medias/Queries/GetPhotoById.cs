using FluentValidation;
using Restaurant.Application.Features.Medias.Models;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Medias.Queries;

public sealed record GetPhotoById(long PhotoId) : IQuery<DishMediaDto>;

// ReSharper disable once UnusedType.Global
public sealed class GetPhotoByIdValidator : AbstractValidator<GetPhotoById>
{
    public GetPhotoByIdValidator()
    {
        RuleFor(x => x.PhotoId)
            .GreaterThan(0)
            .WithMessage("PhotoId most be greaten then 0");
    }
}

internal sealed class GetPhotoByIdHandler : IQueryHandler<GetPhotoById, DishMediaDto>
{
    private readonly IDishMediaRepository _dishMediaRepository;
    private readonly DishMediaMapper _dishMediaMapper;

    public GetPhotoByIdHandler(IDishMediaRepository dishMediaRepository, DishMediaMapper dishMediaMapper)
    {
        _dishMediaRepository = dishMediaRepository;
        _dishMediaMapper = dishMediaMapper;
    }

    public async Task<DishMediaDto> Handle(GetPhotoById request, CancellationToken cancellationToken)
    {
        var dishMedia = await _dishMediaRepository.GetByIdAsync(request.PhotoId, cancellationToken);
        if (dishMedia is null)
            throw new ResourceNotFoundException(MediaErrors.NotFound);

        return _dishMediaMapper.Map(dishMedia);
    }
}