using Ardalis.Specification.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.Application.Features.AuditLogs.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : RepositoryBase<AuditLog>, IAuditRepository
{
    public AuditLogRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}