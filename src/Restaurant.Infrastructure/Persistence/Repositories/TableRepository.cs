using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Tables.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class TableRepository(AppDbContext dbContext) : RepositoryBase<Table>(dbContext), ITableRepository;
