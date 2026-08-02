using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Restaurant.Application.Features.Dishes.Models;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Dishes.Queries;

public sealed record GetDishById(long Id) : IQuery<DishDto>;

// ReSharper disable once UnusedType.Global
public class GetDishByIdValidator : AbstractValidator<GetDishById>
{
    public GetDishByIdValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}

internal sealed class GetDishByIdHandler : IQueryHandler<GetDishById, DishDto>
{
    private readonly IDishRepository _dishRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly DishMapper _mapper;

    public GetDishByIdHandler(IDishRepository dishRepository, DishMapper mapper, IMemoryCache memoryCache)
    {
        _dishRepository = dishRepository;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }

    public async Task<DishDto> Handle(GetDishById request, CancellationToken cancellationToken)
    {
        try
        {
            var dish = await _dishRepository.GetByIdAsync(request.Id, cancellationToken);

            if (dish is null || !dish.IsActive)
                throw new ResourceNotFoundException(DishErrors.NotFound);

            return _mapper.Map(dish);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}