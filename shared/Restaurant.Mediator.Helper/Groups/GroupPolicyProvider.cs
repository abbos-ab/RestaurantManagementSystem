using Auth.Shared.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Restaurant.Mediator.Helper.Groups;

public class GroupPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _options;

    public GroupPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _options = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyName.Group, StringComparison.OrdinalIgnoreCase))
        {
            var groupString = policyName[PolicyName.Group.Length..];
            var groups = groupString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new GroupRequirement(groups))
                .Build();

            // Кешируем динамически созданную политику
            _options.AddPolicy(policyName, policy);

            return policy;
        }

        return await base.GetPolicyAsync(policyName);
    }
}

public static class PolicyName
{
    public const string Group = "Group:";
}
