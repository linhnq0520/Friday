namespace Friday.Modules.Admin.Application.Authorization;

/// <summary>Role code values (must match <c>admin.roles.Code</c>) used in JWT role claims and authorization policies.</summary>
public static class AdminRoleCodes
{
    public const string Administrator = "ADMIN";
    public const string Support = "SUPPORT";
    public const string Viewer = "VIEWER";
}
