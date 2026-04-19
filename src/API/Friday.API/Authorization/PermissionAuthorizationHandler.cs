using System.Security.Claims;
using Friday.Modules.Admin.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Friday.API.Authorization;

public sealed class PermissionAuthorizationHandler(
    IEffectivePermissionResolver permissionResolver,
    IHttpContextAccessor httpContextAccessor
) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        string? sub =
            context.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out int userId))
        {
            return;
        }

        CancellationToken cancellationToken =
            httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

        IReadOnlySet<string> granted = await permissionResolver.GetEffectivePermissionKeysAsync(
            userId,
            cancellationToken
        );

        if (EffectivePermissionSatisfaction.IsSatisfied(granted, requirement.PermissionKey))
        {
            context.Succeed(requirement);
        }
    }
}
