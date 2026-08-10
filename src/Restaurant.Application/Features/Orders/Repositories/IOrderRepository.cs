using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Repositories;

public interface IOrderRepository : IRepositoryBase<Order>
{
    Task<int> GetTodayOrderCountAsync(
    DateTime today,
    CancellationToken cancellationToken);

    Task<decimal> GetTodayRevenueAsync(
        DateTime today,
        CancellationToken cancellationToken);

    Task<int> GetCompletedOrderCountAsync(
        CancellationToken cancellationToken);
}