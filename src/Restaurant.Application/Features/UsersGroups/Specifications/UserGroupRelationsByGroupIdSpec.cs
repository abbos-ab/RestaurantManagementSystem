using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Restaurant.Application.Features.UsersGroups.Specifications;

public sealed class UserGroupRelationsByGroupIdSpec : Specification<UserGroupRelation>
{
    public UserGroupRelationsByGroupIdSpec(long groupId)
    {
        Query.Where(x => x.GroupId == groupId);
    }
}
