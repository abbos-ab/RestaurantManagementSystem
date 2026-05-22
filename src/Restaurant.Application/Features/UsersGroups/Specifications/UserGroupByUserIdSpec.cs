using Ardalis.Specification;
using Restaurant.Domain.Entities;

namespace Auth.Application.UsersGroups.Specifications;

public sealed record UserGroupByUserIdParams
{
    public required long UserId { get; init; }

    public required bool AsNoTracking { get; init; }

    public bool IncludeUser { get; init; }

    public bool IncludeGroup { get; init; }
}

public sealed class UserGroupByUserIdSpec : Specification<UserGroupRelation>
{
    public UserGroupByUserIdSpec(UserGroupByUserIdParams @params)
    {
        if (@params.AsNoTracking)
            Query.AsNoTracking();

        if (@params.IncludeUser)
            Query.Include(x => x.User);

        if (@params.IncludeGroup)
            Query.Include(x => x.Group);

        Query.Where(x => x.UserId == @params.UserId);
    }
}
