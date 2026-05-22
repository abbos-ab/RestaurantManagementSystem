using Restaurant.Application.Features.OrderHistories.Models;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Mediator.Helper.CQRS.Queries;
using Restaurant.Mediator.Helper.Exceptions;

namespace Restaurant.Application.Features.OrderHistories.Queries;

public sealed record GetOrderHistoryById(long Id) : IQuery<OrderHistoryDto>;

internal sealed class GetOrderHistoryByIdHandler : IQueryHandler<GetOrderHistoryById, OrderHistoryDto>
{
    private readonly IOrderHistoryRepository _repository;
    private readonly OrderHistoryMapper _mapper;

    public GetOrderHistoryByIdHandler(
        IOrderHistoryRepository repository,
        OrderHistoryMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<OrderHistoryDto> Handle(GetOrderHistoryById request, CancellationToken cancellationToken)
    {
        var orderHistory = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (orderHistory is null)
            throw new BusinessLogicException(OrderHistoryErrors.NotFound);

        return _mapper.Map(orderHistory);
    }
}