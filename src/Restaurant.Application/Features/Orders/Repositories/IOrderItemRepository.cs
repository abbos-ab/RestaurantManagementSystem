using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Orders.Repositories;

public interface IOrderItemRepository :  IRepositoryBase<OrderItem>
{
    Task<int> CountByStatusAsync(
    OrderItemStatus status,
    CancellationToken cancellationToken);
}