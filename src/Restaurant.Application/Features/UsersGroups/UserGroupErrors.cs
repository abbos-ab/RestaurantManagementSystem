using Restaurant.Mediator.Helper.Common.Models;

namespace Restaurant.Application.Features.UsersGroups;

public static class UserGroupErrors
{
    public static readonly Error NotFound = new(
        "UserGroup.NotFound",
        "User group relation was not found."
    );

    public static class Group
    {
        public static readonly Error NotFound = new(
            "UserGroup.Group.NotFound",
            "Group was not found."
        );
    }
}