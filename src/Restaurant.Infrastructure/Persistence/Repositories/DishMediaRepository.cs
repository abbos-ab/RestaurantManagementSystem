using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class DishMediaRepository(AppDbContext dbContext) 
    : RepositoryBase<DishMedia>(dbContext), IDishMediaRepository;