using Ardalis.Specification;
using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.Common.Models;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Extensions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.OrderHistories.Queries;

public sealed record GetAllOrderHistoriesQuery(PaginationInfo PaginationInfo)
    : IQuery<PaginatedResult<OrderHistoryDto>>;

internal sealed class GetAllOrderHistoriesQueryHandler
    : IQueryHandler<GetAllOrderHistoriesQuery, PaginatedResult<OrderHistoryDto>>
{
    private readonly IOrderHistoryRepository _historyRepository;
    private readonly OrderHistoryMapper _historyMapper;

    public GetAllOrderHistoriesQueryHandler(IOrderHistoryRepository historyRepository, OrderHistoryMapper historyMapper)
    {
        _historyRepository = historyRepository;
        _historyMapper = historyMapper;
    }

    public async Task<PaginatedResult<OrderHistoryDto>> Handle(GetAllOrderHistoriesQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<OrderHistory>();
        spec.Query
            .Include(x => x.Order)
            .Include(x => x.User)
            .WithPagination(request.PaginationInfo)
            .OrderBy(x => x.Id);
        
        var orderHistories = await _historyRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _historyRepository.CountAsync(spec, cancellationToken);

        var mapperOrderHistory = _historyMapper.Map(orderHistories);

        return new PaginatedResult<OrderHistoryDto>(mapperOrderHistory, totalCount);
    }
}