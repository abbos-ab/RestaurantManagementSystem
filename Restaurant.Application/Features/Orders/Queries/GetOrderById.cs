using FluentValidation;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Shared.CQRS.Queries;
using Restaurant.Shared.Exceptions;

namespace Restaurant.Application.Features.Orders.Queries;

public sealed record GetOrderById(long Id) : IQuery<OrderDto>;

// ReSharper disable once UnusedType.Global
public sealed class GetOrderByIdValidator : AbstractValidator<GetOrderById>
{
    public GetOrderByIdValidator()
    {
        RuleFor(order => order.Id)
            .GreaterThan(0)
            .WithMessage("OrderId must be greater than 0");
    }
}

internal sealed class GetOrderByIdHandler
    : IQueryHandler<GetOrderById, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly OrderMapper _mapper;

    public GetOrderByIdHandler(IOrderRepository orderRepository, OrderMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(GetOrderById request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
            throw new BusinessLogicException(OrderErrors.NotFound);

        return _mapper.Map(order);
    }
}