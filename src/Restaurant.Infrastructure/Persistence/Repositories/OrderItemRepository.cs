using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Orders.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class OrderItemRepository(AppDbContext dbContext) : RepositoryBase<OrderItem>(dbContext), IOrderItemRepository;