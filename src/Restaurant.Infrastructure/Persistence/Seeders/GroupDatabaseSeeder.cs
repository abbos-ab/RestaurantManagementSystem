using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;
using Restaurant.Mediator.Helper.Common.Extensions;
using Restaurant.Mediator.Helper.Groups;

namespace Restaurant.Infrastructure.Persistence.Seeders;

public class GroupDatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public GroupDatabaseSeeder(AppDbContext dbContext, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task SeedAsync()
    {
        var hasGroups = await _dbContext.Groups.AnyAsync();

        if (hasGroups)
            return;

        var createdDate = _timeProvider.GetLocalDateTimeNowKindUtc();

        var newGroups = new List<Group>
        {
            new()
            {
                Name = GroupNames.Administrators,
                Description = "Full access to all system features and management functions",
                CreatedAt = createdDate,
            },

            new()
            {
                Name = GroupNames.Waiters,
                Description = "Responsible for serving customers, managing orders, and handling tables",
                CreatedAt = createdDate,
            },

            new()
            {
                Name = GroupNames.Chefs,
                Description = "Responsible for preparing meals and managing kitchen operations",
                CreatedAt = createdDate,
            },

            new()
            {
                Name = GroupNames.Customers,
                Description = "Regular users who can browse menus, place orders, and make reservations",
                CreatedAt = createdDate,
            },
        };

        await _dbContext.Groups.AddRangeAsync(newGroups);
        await _dbContext.SaveChangesAsync();
    }
}