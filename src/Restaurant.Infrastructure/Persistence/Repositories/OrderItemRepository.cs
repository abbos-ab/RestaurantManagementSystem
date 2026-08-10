using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class OrderItemRepository(AppDbContext dbContext) : RepositoryBase<OrderItem>(dbContext), IOrderItemRepository
{
    public async Task<int> CountByStatusAsync(
        OrderItemStatus status,
        CancellationToken cancellationToken)
    {
        return await dbContext.OrderItems
            .CountAsync(x => x.Status == status,
                cancellationToken);
    }
}