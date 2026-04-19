namespace Friday.Modules.Admin.Application.Authorization;

/// <summary>ASP.NET Core policy names for permission-based authorization.</summary>
public static class AdminPermissionEndpointPolicy
{
    public const string Prefix = "Permission:";

    /// <summary>Builds policy name understood by <c>PermissionPolicyProvider</c>.</summary>
    public static string ForPermission(string permissionKey) => Prefix + permissionKey;
}
