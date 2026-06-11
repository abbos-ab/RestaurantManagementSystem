using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.Medias.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal sealed class DishMediaRelationRepository(AppDbContext dbContext) 
    : RepositoryBase<DishMediaRelation>(dbContext), IDishMediaRelationRepository;