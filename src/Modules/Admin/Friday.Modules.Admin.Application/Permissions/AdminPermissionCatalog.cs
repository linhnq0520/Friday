using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

namespace Friday.Modules.Admin.Application.Permissions;

/// <summary>
/// Code-defined permission vocabulary for the Admin module. New capabilities are added here (and released),
/// not invented row-by-row through a generic API — that keeps the catalog bounded and reviewable.
/// Other modules can expose their own static catalogs the same way.
/// </summary>
public static class AdminPermissionCatalog
{
    public sealed record Definition(
        string Module,
        string Resource,
        PermissionAccessLevel AccessLevel,
        string Name,
        string Description
    );

    /// <summary>Full set of Admin permissions (module + resource + coarse Read/Manage tier).</summary>
    public static readonly IReadOnlyList<Definition> All =
    [
        new(
            "admin",
            "dashboard",
            PermissionAccessLevel.Read,
            "View dashboard",
            "Open the main dashboard."
        ),
        new(
            "admin",
            "users",
            PermissionAccessLevel.Read,
            "View users",
            "List and read user profiles."
        ),
        new(
            "admin",
            "users",
            PermissionAccessLevel.Manage,
            "Manage users",
            "Create, update, and deactivate users."
        ),
        new(
            "admin",
            "roles",
            PermissionAccessLevel.Read,
            "View roles",
            "List roles and assigned permissions."
        ),
        new(
            "admin",
            "roles",
            PermissionAccessLevel.Manage,
            "Manage roles",
            "Create or update roles and assign permissions."
        ),
        new(
            "admin",
            "permissions",
            PermissionAccessLevel.Read,
            "View permissions",
            "List the permission catalog."
        ),
        new(
            "admin",
            "permissions",
            PermissionAccessLevel.Manage,
            "Manage catalog sync",
            "Sync the permission catalog from application definitions (bootstrap / ops)."
        ),
    ];
}
