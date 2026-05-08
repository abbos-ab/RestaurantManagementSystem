using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Queries;

public sealed record GetOrdersByStatusQuery(OrderStatus Status) : IQuery<List<OrderDto>>;

internal sealed class GetOrdersByStatusQueryHandler
    : IQueryHandler<GetOrdersByStatusQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderMapper _mapper;

    public GetOrdersByStatusQueryHandler(IOrderRepository orderRepository, OrderMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByStatusQuery request, CancellationToken cancellationToken)
    {
        var spec = new OrderByStatusSpec(request.Status);

        var orders = await _orderRepository.ListAsync(spec, cancellationToken);

        return _mapper.Map(orders);
    }
}