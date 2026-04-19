namespace Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

/// <summary>
/// Coarse access tier per resource to avoid CRUD-per-endpoint permission explosion.
/// <see cref="Read"/> = view/list; <see cref="Manage"/> = create, update, delete, and workflow for that resource.
/// </summary>
public enum PermissionAccessLevel
{
    Read = 0,
    Manage = 1,
}
