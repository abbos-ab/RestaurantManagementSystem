using FluentValidation;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.Tables.Queries;

public sealed record GetTableByIdQuery(long TableId) : IQuery<TableDto>;

// ReSharper disable once UnusedType.Global
public sealed class GetTableByIdQueryValidator : AbstractValidator<GetTableByIdQuery>
{
    public GetTableByIdQueryValidator()
    {
        RuleFor(t => t.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");
    }
}

internal sealed class GetTableByIdQueryHandler : IQueryHandler<GetTableByIdQuery, TableDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly TableMapper _tableMapper;

    public GetTableByIdQueryHandler(
        ITableRepository tableRepository,
        TableMapper tableMapper)
    {
        _tableRepository = tableRepository;
        _tableMapper = tableMapper;
    }

    public async Task<TableDto> Handle(GetTableByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken);

        if (entity is null)
            throw new BusinessLogicException(TableErrors.NotFound);

        return _tableMapper.Map(entity);
    }
}