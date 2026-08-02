using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;
using Restaurant.Mediator.Helper.Common.Extensions;

namespace Restaurant.Infrastructure.Persistence.Seeders;


public sealed class TableDatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public TableDatabaseSeeder(
        AppDbContext dbContext,
        TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task SeedAsync()
    {
        var hasTables = await _dbContext.Tables.AnyAsync();

        if (hasTables)
            return;

        var createdAt = _timeProvider.GetLocalDateTimeNowKindUtc();

        var tables = new List<Table>
        {
            new()
            {
                Number = 1,
                Capacity = 2,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Number = 2,
                Capacity = 2,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Number = 3,
                Capacity = 4,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            
            new()
            {
                Number = 4,
                Capacity = 4,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },

            new()
            {
                Number = 5,
                Capacity = 6,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            
            new()
            {
                Number = 6,
                Capacity = 8,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            },
            
            new()
            {
                Number = 7,
                Capacity = 10,
                Status = TableStatus.Available,
                CreatedAt = createdAt,
                UpdatedAt = createdAt
            }
        };

        await _dbContext.Tables.AddRangeAsync(tables);

        await _dbContext.SaveChangesAsync();
    }
}