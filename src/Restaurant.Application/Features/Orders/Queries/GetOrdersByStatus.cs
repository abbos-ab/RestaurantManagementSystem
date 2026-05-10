using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Application.Features.Orders.Specifications;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Queries;

namespace Restaurant.Application.Features.Orders.Queries;

public sealed record GetOrdersByStatus(OrderStatus Status) : IQuery<List<OrderDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetOrdersByStatusValidator : AbstractValidator<GetOrdersByStatus>
{
    public GetOrdersByStatusValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}

internal sealed class GetOrdersByStatusHandler
    : IQueryHandler<GetOrdersByStatus, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderMapper _mapper;

    public GetOrdersByStatusHandler(IOrderRepository orderRepository, OrderMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderDto>> Handle(GetOrdersByStatus request, CancellationToken cancellationToken)
    {
        var spec = new OrderByStatusSpec(request.Status);
        var orders = await _orderRepository.ListAsync(spec, cancellationToken);

        return _mapper.Map(orders);
    }
}