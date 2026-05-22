using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Authentications.Interfaces;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
