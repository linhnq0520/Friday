namespace Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

/// <summary>Canonical permission key format: <c>{module}:{resource}:read|manage</c>.</summary>
public static class PermissionKeys
{
    public static string Format(string module, string resource, PermissionAccessLevel accessLevel)
    {
        string level = accessLevel == PermissionAccessLevel.Manage ? "manage" : "read";
        return $"{module}:{resource}:{level}";
    }

    /// <summary>Parses a key produced by <see cref="Format"/> (exactly three segments).</summary>
    public static bool TryParse(
        string permissionKey,
        out string module,
        out string resource,
        out string level
    )
    {
        module = string.Empty;
        resource = string.Empty;
        level = string.Empty;
        if (string.IsNullOrWhiteSpace(permissionKey))
        {
            return false;
        }

        string[] parts = permissionKey.Split(':', StringSplitOptions.None);
        if (parts.Length != 3)
        {
            return false;
        }

        level = parts[2];
        if (level is not ("read" or "manage"))
        {
            return false;
        }

        module = parts[0];
        resource = parts[1];
        return true;
    }
}
