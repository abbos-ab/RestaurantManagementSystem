using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Categories.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(AppDbContext dbContext) : RepositoryBase<Category>(dbContext), ICategoryRepository;