using Ardalis.Specification;
using Auth.Domain.Models;

namespace Restaurant.Application.Features.UsersGroups.Repositories;

public interface IGroupRepository : IRepositoryBase<Group>
{
    Task<Group?> GetGroupByNameAsync(string groupName);
}
