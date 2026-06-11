using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.UsersGroups.Repositories;

public interface IGroupRepository : IRepositoryBase<Group>
{
    Task<Group?> GetGroupByNameAsync(string groupName);
}
