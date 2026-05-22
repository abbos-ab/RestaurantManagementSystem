using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.OrderHistories.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class OrderHistoryRepository(AppDbContext dbContext)
    : RepositoryBase<OrderHistory>(dbContext), IOrderHistoryRepository;