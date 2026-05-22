using Microsoft.AspNetCore.Authorization;

namespace Auth.Shared.Groups;

public class GroupRequirement : IAuthorizationRequirement
{
    public IReadOnlyList<string> RequiredGroups { get; }

    public GroupRequirement(IEnumerable<string> requiredGroups)
    {
        RequiredGroups = [.. requiredGroups];
    }
}
