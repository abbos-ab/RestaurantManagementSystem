using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.Tables.Repositories;

public interface ITableRepository : IRepositoryBase<Table>
{
    Task<int> GetTotalTablesAsync(
    CancellationToken cancellationToken);

    Task<int> GetOccupiedTablesAsync(
        CancellationToken cancellationToken);

    Task<int> GetAvailableTablesAsync(
        CancellationToken cancellationToken);
}