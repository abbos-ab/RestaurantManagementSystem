using Ardalis.Specification;
using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Application.Features.UsersGroups.Specifications;

public sealed class UsersByIdsSpec : Specification<User>
{
    public List<long> UserIds { get; }

    public UsersByIdsSpec(List<long> userIds, bool asNoTracking = false)
    {
        UserIds = userIds;

        if (asNoTracking)
            Query.AsNoTracking();

        Query.Where(x => userIds.Contains(x.Id));
    }
}

