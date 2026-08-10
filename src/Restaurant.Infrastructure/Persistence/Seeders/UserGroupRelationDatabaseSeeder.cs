using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Seeders.Interfaces;
using Restaurant.Mediator.Helper.Groups;

namespace Restaurant.Infrastructure.Persistence.Seeders;

public sealed class UserGroupRelationDatabaseSeeder : IDatabaseSeeder
{
    private readonly AppDbContext _dbContext;

    public UserGroupRelationDatabaseSeeder(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        var hasRelations = await _dbContext.UserGroupRelations.AnyAsync();

        if (hasRelations)
            return;

        var admin = await _dbContext.Users.FindAsync(1L);
        var waiter1 = await _dbContext.Users.FindAsync(2L);
        var waiter2 = await _dbContext.Users.FindAsync(3L);
        var chef1 = await _dbContext.Users.FindAsync(4L);
        var chef2 = await _dbContext.Users.FindAsync(5L);
        
        if (admin is null ||
            waiter1 is null ||
            waiter2 is null ||
            chef1 is null ||
            chef2 is null)
        {
            throw new InvalidOperationException(
                "Users must be seeded before UserGroupRelationSeeder");
        }

        var administrators = await _dbContext.Groups
            .FirstAsync(x => x.Name == GroupNames.Administrators);

        var waiters = await _dbContext.Groups
            .FirstAsync(x => x.Name == GroupNames.Waiters);

        var chefs = await _dbContext.Groups
            .FirstAsync(x => x.Name == GroupNames.Chefs);
        
        var relations = new List<UserGroupRelation>
        {
            new()
            {
                UserId = admin.Id,
                GroupId = administrators.Id
            },

            new()
            {
                UserId = waiter1.Id,
                GroupId = waiters.Id
            },

            new()
            {
                UserId = waiter2.Id,
                GroupId = waiters.Id
            },

            new()
            {
                UserId = chef1.Id,
                GroupId = chefs.Id
            },

            new()
            {
                UserId = chef2.Id,
                GroupId = chefs.Id
            }
        };

        await _dbContext.UserGroupRelations.AddRangeAsync(relations);

        await _dbContext.SaveChangesAsync();
    }
}