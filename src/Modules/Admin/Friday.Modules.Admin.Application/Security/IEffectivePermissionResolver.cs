namespace Friday.Modules.Admin.Application.Security;

/// <summary>
/// Resolves the effective permission keys for a user from persisted role–right assignments (union across active roles).
/// Implementations are typically scoped per HTTP request so results can be memoized for that request.
/// </summary>
public interface IEffectivePermissionResolver
{
    /// <summary>Returns distinct permission keys (e.g. <c>admin:users:read</c>) the user has via roles.</summary>
    Task<IReadOnlySet<string>> GetEffectivePermissionKeysAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
