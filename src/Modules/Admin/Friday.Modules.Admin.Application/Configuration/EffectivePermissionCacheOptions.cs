namespace Friday.Modules.Admin.Application.Configuration;

public sealed class EffectivePermissionCacheOptions
{
    public const string SectionName = "Admin:EffectivePermissionCache";

    /// <summary>Absolute TTL for cached permission key sets per user (minutes). Default 60.</summary>
    public int GrantListTtlMinutes { get; set; } = 60;
}
