using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Inventories.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class InventoryRepository(AppDbContext dbContext) : RepositoryBase<Inventory>(dbContext), IInventoryRepository;