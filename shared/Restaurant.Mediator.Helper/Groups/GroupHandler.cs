using Auth.Shared.Groups;
using Microsoft.AspNetCore.Authorization;

namespace Restaurant.Mediator.Helper.Groups;

public class GroupHandler : AuthorizationHandler<GroupRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupRequirement requirement)
    {
        var userGroups = context
            .User
            .FindFirst(CustomClaimTypes.Groups)?
            .Value
            .Split(",", StringSplitOptions.RemoveEmptyEntries);

        if (userGroups is null || userGroups.Length == 0)
            return Task.CompletedTask;

        if (requirement.RequiredGroups.Any(rg => userGroups.Contains(rg)))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
