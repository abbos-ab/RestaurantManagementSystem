using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(AppDbContext dbContext) : RepositoryBase<Order>(dbContext), IOrderRepository;