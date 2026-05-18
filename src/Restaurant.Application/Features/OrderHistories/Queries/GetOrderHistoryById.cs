using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.OrderHistories.Queries;

public sealed record GetOrderHistoryByIdQuery(long Id) : IQuery<OrderHistoryDto>;

internal sealed class GetOrderHistoryByIdQueryHandler : IQueryHandler<GetOrderHistoryByIdQuery, OrderHistoryDto>
{
    private readonly IOrderHistoryRepository _repository;
    private readonly OrderHistoryMapper _mapper;

    public GetOrderHistoryByIdQueryHandler(
        IOrderHistoryRepository repository,
        OrderHistoryMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OrderHistoryDto> Handle(GetOrderHistoryByIdQuery request, CancellationToken cancellationToken)
    {
        var orderHistory = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (orderHistory is null)
            throw new BusinessLogicException(OrderHistoryErrors.NotFound);

        return _mapper.Map(orderHistory);
    }
}