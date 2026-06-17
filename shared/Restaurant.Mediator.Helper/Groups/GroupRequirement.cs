using Microsoft.AspNetCore.Authorization;

namespace Restaurant.Mediator.Helper.Groups;

public class GroupRequirement : IAuthorizationRequirement
{
    public IReadOnlyList<string> RequiredGroups { get; }

    public GroupRequirement(IEnumerable<string> requiredGroups)
    {
        RequiredGroups = [.. requiredGroups];
    }
}
