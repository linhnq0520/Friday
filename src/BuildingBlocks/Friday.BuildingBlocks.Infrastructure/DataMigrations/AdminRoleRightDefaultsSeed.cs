using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

namespace Friday.BuildingBlocks.Infrastructure.DataMigrations;

/// <summary>
/// Default roles and role–permission links. Permission rows match <see cref="Friday.Modules.Admin.Application.Permissions.AdminPermissionCatalog"/>.
/// </summary>
internal static class AdminRoleRightDefaultsSeed
{
    internal readonly record struct RoleRow(string Code, string Name);

    internal readonly record struct PermissionRef(
        string Module,
        string Resource,
        PermissionAccessLevel AccessLevel
    );

    internal readonly record struct RolePermissionsRow(
        string RoleCode,
        IReadOnlyList<PermissionRef> Permissions
    );

    internal static readonly IReadOnlyList<RoleRow> Roles =
    [
        new("ADMIN", "Administrator"),
        new("SUPPORT", "Support"),
        new("VIEWER", "Viewer"),
        new("USER", "User"),
    ];

    internal static readonly IReadOnlyList<RolePermissionsRow> RolePermissions =
    [
        new(
            "ADMIN",
            [
                new("admin", "dashboard", PermissionAccessLevel.Read),
                new("admin", "users", PermissionAccessLevel.Read),
                new("admin", "users", PermissionAccessLevel.Manage),
                new("admin", "roles", PermissionAccessLevel.Read),
                new("admin", "roles", PermissionAccessLevel.Manage),
                new("admin", "permissions", PermissionAccessLevel.Read),
                new("admin", "permissions", PermissionAccessLevel.Manage),
            ]
        ),
        new(
            "SUPPORT",
            [
                new("admin", "dashboard", PermissionAccessLevel.Read),
                new("admin", "users", PermissionAccessLevel.Read),
                new("admin", "users", PermissionAccessLevel.Manage),
                new("admin", "roles", PermissionAccessLevel.Read),
                new("admin", "permissions", PermissionAccessLevel.Read),
            ]
        ),
        new(
            "VIEWER",
            [
                new("admin", "dashboard", PermissionAccessLevel.Read),
                new("admin", "users", PermissionAccessLevel.Read),
                new("admin", "roles", PermissionAccessLevel.Read),
                new("admin", "permissions", PermissionAccessLevel.Read),
            ]
        ),
    ];
}
