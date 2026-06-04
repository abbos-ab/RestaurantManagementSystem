using Ardalis.Specification.EntityFrameworkCore;
using Restaurant.Application.Features.UsersGroups.Repositories;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence.Repositories;

internal class UserGroupRepository(AppDbContext dbContext)
    : RepositoryBase<UserGroupRelation>(dbContext), IUserGroupRepository;