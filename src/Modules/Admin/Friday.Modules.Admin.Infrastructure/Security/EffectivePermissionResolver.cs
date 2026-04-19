using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Application.Security;
using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;
using Friday.Modules.Admin.Domain.Aggregates.RoleAggregate;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Friday.Modules.Admin.Infrastructure.Security;

/// <summary>
/// Loads union of permission keys from active roles (scoped: memoized per request / scope).
/// </summary>
public sealed class EffectivePermissionResolver(FridayDbContext dbContext)
    : IEffectivePermissionResolver
{
    private int? _cachedUserId;
    private IReadOnlySet<string>? _cachedKeys;

    public async Task<IReadOnlySet<string>> GetEffectivePermissionKeysAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        if (_cachedUserId == userId && _cachedKeys is not null)
        {
            return _cachedKeys;
        }

        List<string> keys = await dbContext
            .Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => r)
            .Where(r => r.IsActive)
            .Join(dbContext.Set<RoleRight>(), r => r.Id, rr => rr.RoleId, (r, rr) => rr)
            .Join(dbContext.Set<Right>(), rr => rr.RightId, right => right.Id, (rr, right) => right)
            .Select(right => PermissionKeys.Format(right.Module, right.Resource, right.AccessLevel))
            .Distinct()
            .ToListAsync(cancellationToken);

        HashSet<string> set = keys.ToHashSet(StringComparer.Ordinal);
        _cachedUserId = userId;
        _cachedKeys = set;
        return set;
    }
}
