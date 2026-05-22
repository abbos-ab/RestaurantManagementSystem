using Ardalis.Specification;
using Auth.Domain.Models;

namespace Restaurant.Application.Features.UsersGroups.Specifications;

public sealed class GroupByIdSpec : Specification<Group>
{
    public long GroupId { get;}
    public GroupByIdSpec(long groupId, bool asNoTracking = false)
    {
        GroupId = groupId;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => x.Id == GroupId);
    }
}

