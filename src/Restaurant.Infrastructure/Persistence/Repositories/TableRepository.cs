using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class TableRepository
    : RepositoryBase<Table>, ITableRepository
{
    private readonly AppDbContext _context;


    public TableRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }


    public async Task<int> GetTotalTablesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tables
            .CountAsync(cancellationToken);
    }


    public async Task<int> GetOccupiedTablesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tables
            .CountAsync(
                x => x.Status == TableStatus.Occupied,
                cancellationToken);
    }


    public async Task<int> GetAvailableTablesAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tables
            .CountAsync(
                x => x.Status == TableStatus.Available,
                cancellationToken);
    }
}