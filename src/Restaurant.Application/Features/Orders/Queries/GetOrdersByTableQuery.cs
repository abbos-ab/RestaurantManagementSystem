using Ardalis.Specification;
using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Application.Features.Orders.Queries;

public record GetOrdersByTableQuery(long TableId) : IQuery<IEnumerable<OrderDto>>;

// ReSharper disable once UnusedType.Global
public sealed class GetOrdersByTableQueryValidator : AbstractValidator<GetOrdersByTableQuery>
{
    public GetOrdersByTableQueryValidator()
    {
        RuleFor(r => r.TableId)
            .GreaterThan(0)
            .WithMessage("TableId must be greater than 0");
    }
}

internal sealed class GetOrdersByTableQueryHandler : IQueryHandler<GetOrdersByTableQuery, IEnumerable<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderMapper _orderMapper;

    public GetOrdersByTableQueryHandler(IOrderRepository orderRepository, OrderMapper orderMapper)
    {
        _orderRepository = orderRepository;
        _orderMapper = orderMapper;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersByTableQuery request, CancellationToken cancellationToken)
    {
        var spec = new DbSpecification<Order>();
        spec.Query.Where(x => x.TableId == request.TableId);

        var orders = await _orderRepository.ListAsync(spec, cancellationToken);
        if (!orders.Any())
            throw new ResourceNotFoundException(OrderErrors.NotFound);

        return _orderMapper.Map(orders);
    }
}