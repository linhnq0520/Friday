namespace Friday.Modules.Admin.Application.Security;

/// <summary>
/// Coordinates invalidation of cached effective permission key sets (e.g. after role-rights change).
/// </summary>
public interface IEffectivePermissionGrantCacheCoordinator
{
    /// <summary>Current cache generation; entries from an older generation are treated as stale.</summary>
    long CurrentGeneration { get; }

    /// <summary>Invalidate all cached grant lists (logical clear without enumerating cache keys).</summary>
    void InvalidateAllGrants();
}
