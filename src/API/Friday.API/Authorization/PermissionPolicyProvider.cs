using Friday.Modules.Admin.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Friday.API.Authorization;

/// <summary>
/// Builds policies for names <c>Permission:{permissionKey}</c> so endpoints stay declarative without registering every key at startup.
/// </summary>
public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (
            policyName.StartsWith(AdminPermissionEndpointPolicy.Prefix, StringComparison.Ordinal)
            && policyName.Length > AdminPermissionEndpointPolicy.Prefix.Length
        )
        {
            string key = policyName[AdminPermissionEndpointPolicy.Prefix.Length..];
            AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(key))
                .Build();
            return policy;
        }

        return await base.GetPolicyAsync(policyName).ConfigureAwait(false);
    }
}
