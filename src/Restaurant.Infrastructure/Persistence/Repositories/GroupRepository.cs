using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class GroupRepository : RepositoryBase<Group>, IGroupRepository
{
    private readonly AppDbContext _context;

    public GroupRepository(AppDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<Group?> GetGroupByNameAsync(string groupName)
    {
        return await _context.Groups.FirstOrDefaultAsync(x => x.Name == groupName);
    }
}