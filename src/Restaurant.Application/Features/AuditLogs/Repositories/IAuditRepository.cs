using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.AuditLogs.Repositories;

public interface IAuditRepository : IRepositoryBase<AuditLog>;