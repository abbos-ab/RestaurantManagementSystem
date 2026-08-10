using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository
    : RepositoryBase<Order>, IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<int> GetTodayOrderCountAsync(
        DateTime today,
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .CountAsync(x => x.CreatedAt >= today, cancellationToken);
    }

    public async Task<decimal> GetTodayRevenueAsync(
        DateTime today,
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Where(x => x.CreatedAt >= today)
            .SumAsync(x => x.TotalPrice, cancellationToken);
    }

    public async Task<int> GetCompletedOrderCountAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Orders
            .CountAsync(x => x.Status == OrderStatus.Completed,
                cancellationToken);
    }
}