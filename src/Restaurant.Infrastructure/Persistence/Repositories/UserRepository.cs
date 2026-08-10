using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.Users.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository
    : RepositoryBase<User>, IUserRepository
{
    private readonly AppDbContext _context;


    public UserRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }


    public async Task<int> GetActiveWaitersCountAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(x => x.IsActive)
            .Where(x => x.Groups.Any(
                g => g.Name == "Waiters"))
            .CountAsync(cancellationToken);
    }


    public async Task<int> GetActiveChefsCountAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(x => x.IsActive)
            .Where(x => x.Groups.Any(
                g => g.Name == "Chefs"))
            .CountAsync(cancellationToken);
    }
}