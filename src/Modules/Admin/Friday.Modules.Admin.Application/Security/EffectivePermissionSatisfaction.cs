using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

namespace Friday.Modules.Admin.Application.Security;

/// <summary>
/// Permission matching rules: exact key, plus <see cref="PermissionAccessLevel.Manage"/> implies
/// read for the same module and resource.
/// </summary>
public static class EffectivePermissionSatisfaction
{
    /// <summary>Returns true if <paramref name="granted"/> satisfies <paramref name="requiredPermissionKey"/>.</summary>
    public static bool IsSatisfied(IReadOnlySet<string> granted, string requiredPermissionKey)
    {
        if (granted.Count == 0 || string.IsNullOrWhiteSpace(requiredPermissionKey))
        {
            return false;
        }

        if (granted.Contains(requiredPermissionKey))
        {
            return true;
        }

        if (
            !PermissionKeys.TryParse(
                requiredPermissionKey,
                out string module,
                out string resource,
                out string level
            )
        )
        {
            return false;
        }

        if (level != "read")
        {
            return false;
        }

        string manageKey = PermissionKeys.Format(module, resource, PermissionAccessLevel.Manage);
        return granted.Contains(manageKey);
    }
}
