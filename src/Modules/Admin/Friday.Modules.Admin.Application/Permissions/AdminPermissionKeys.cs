using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

namespace Friday.Modules.Admin.Application.Permissions;

/// <summary>Stable permission keys for the Admin API (must match rows in <c>admin.rights</c> / catalog).</summary>
public static class AdminPermissionKeys
{
    public static string DashboardRead =>
        PermissionKeys.Format("admin", "dashboard", PermissionAccessLevel.Read);

    public static string UsersRead =>
        PermissionKeys.Format("admin", "users", PermissionAccessLevel.Read);

    public static string UsersManage =>
        PermissionKeys.Format("admin", "users", PermissionAccessLevel.Manage);

    public static string RolesRead =>
        PermissionKeys.Format("admin", "roles", PermissionAccessLevel.Read);

    public static string RolesManage =>
        PermissionKeys.Format("admin", "roles", PermissionAccessLevel.Manage);

    public static string PermissionsRead =>
        PermissionKeys.Format("admin", "permissions", PermissionAccessLevel.Read);

    public static string PermissionsManage =>
        PermissionKeys.Format("admin", "permissions", PermissionAccessLevel.Manage);

    public static string NotificationTemplatesRead =>
        PermissionKeys.Format("admin", "notification-templates", PermissionAccessLevel.Read);

    public static string NotificationTemplatesManage =>
        PermissionKeys.Format("admin", "notification-templates", PermissionAccessLevel.Manage);
}
