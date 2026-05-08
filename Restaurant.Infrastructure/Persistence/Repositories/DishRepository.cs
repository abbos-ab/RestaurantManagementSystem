using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Dishes.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class DishRepository(AppDbContext dbContext) : RepositoryBase<Dish>(dbContext), IDishRepository;