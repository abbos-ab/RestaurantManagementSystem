using Microsoft.AspNetCore.Authorization;

namespace Restaurant.Mediator.Helper.Groups;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class GroupAuthorizeAttribute : AuthorizeAttribute
{
    public GroupAuthorizeAttribute(params string[] groups)
    {
        Policy = $"{PolicyName.Group}{string.Join(",", groups)}";
    }
}
