using Microsoft.AspNetCore.Authorization;

namespace Friday.API.Authorization;

public sealed class PermissionRequirement(string permissionKey) : IAuthorizationRequirement
{
    public string PermissionKey { get; } =
        permissionKey ?? throw new ArgumentNullException(nameof(permissionKey));
}
