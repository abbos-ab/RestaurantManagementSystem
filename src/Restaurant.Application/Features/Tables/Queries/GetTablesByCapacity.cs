using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Application.Features.Tables.Specifications;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;

namespace Restaurant.Application.Features.Tables.Queries;

public sealed record GetTablesByCapacity(int Capacity, PaginationInfo PaginationInfo)
    : IQuery<PaginatedResult<TableDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetTablesByCapacityValidator : AbstractValidator<GetTablesByCapacity>
{
    public GetTablesByCapacityValidator()
    {
        RuleFor(x => x.Capacity)
            .NotEmpty()
            .WithMessage("Capacity is required");
    }
}

internal sealed class GetTablesByCapacitySpecHandler : IQueryHandler<GetTablesByCapacity, PaginatedResult<TableDto>>
{
    private readonly ITableRepository _tableRepository;
    private readonly TableMapper _tableMapper;

    public GetTablesByCapacitySpecHandler(ITableRepository tableRepository, TableMapper tableMapper)
    {
        _tableRepository = tableRepository;
        _tableMapper = tableMapper;
    }

    public async Task<PaginatedResult<TableDto>> Handle(GetTablesByCapacity request,
        CancellationToken cancellationToken)
    {
        var spec = new TableByCapacitySpec(request.Capacity);
        spec.Query.Include(x => x.Orders)
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.Number);

        var tables = await _tableRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _tableRepository.CountAsync(spec, cancellationToken);
        
        var mappedTable = _tableMapper.Map(tables);
        
        return new PaginatedResult<TableDto>(mappedTable, totalCount);
    }
}