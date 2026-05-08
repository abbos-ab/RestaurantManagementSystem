using Ardalis.Specification;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Shared.Common.Models;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Shared.Exceptions;
using Restaurant.Shared.Persistence;

namespace Restaurant.Application.Features.Orders.Queries;

public sealed record GetAllOrders(PaginationInfo Pagination) : IQuery<PaginatedResult<OrderDto>>;

internal sealed class GetAllOrdersHandler : IQueryHandler<GetAllOrders, PaginatedResult<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderMapper _mapper;

    public GetAllOrdersHandler(IOrderRepository orderRepository, OrderMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<OrderDto>> Handle(GetAllOrders request, CancellationToken cancellationToken)
    {
        var spec = new ReadOnlySpecification<Order>();
        spec.Query
            .Include(x => x.Table)
            .Include(x => x.Waiter)
            .Include(x => x.OrderItems)
            .WithPagination(request.Pagination);

        var orders = await _orderRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _orderRepository.CountAsync(spec, cancellationToken);

        var mapperOrder = _mapper.Map(orders);

        return new PaginatedResult<OrderDto>(mapperOrder, totalCount);
    }
}