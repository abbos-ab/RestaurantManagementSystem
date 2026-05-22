using Ardalis.Specification;
using Restaurant.Application.Features.Tables.Models;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Tables.Queries;

public sealed record GetAllTablesQuery(PaginationInfo PaginationInfo) : IQuery<PaginatedResult<TableDto>>;

internal sealed class GetAllTablesQueryHandler : IQueryHandler<GetAllTablesQuery, PaginatedResult<TableDto>>
{
    private readonly ITableRepository _repository;
    private readonly TableMapper _mapper;

    public GetAllTablesQueryHandler(
        ITableRepository repository,
        TableMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<TableDto>> Handle(GetAllTablesQuery request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Table>();
        spec.Query
            .Include(x => x.Orders)
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.Number);
        
        var tables = await _repository.ListAsync(spec, cancellationToken);
        var totalCount = await _repository.CountAsync(spec, cancellationToken);
        
        var mapperTables = _mapper.Map(tables);
        
        return new PaginatedResult<TableDto>(mapperTables, totalCount);
    }
}