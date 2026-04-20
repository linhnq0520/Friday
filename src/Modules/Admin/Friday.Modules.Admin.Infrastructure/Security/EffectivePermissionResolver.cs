using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Application.Configuration;
using Friday.Modules.Admin.Application.Security;
using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;
using Friday.Modules.Admin.Domain.Aggregates.RoleAggregate;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Friday.Modules.Admin.Infrastructure.Security;

/// <summary>
/// Loads union of permission keys from active roles. Results are cached per user (in-process)
/// with a configurable absolute TTL; cache is logically cleared when role rights change.
/// </summary>
public sealed class EffectivePermissionResolver(
    FridayDbContext dbContext,
    IMemoryCache memoryCache,
    IOptions<EffectivePermissionCacheOptions> cacheOptions,
    IEffectivePermissionGrantCacheCoordinator grantCacheCoordinator
) : IEffectivePermissionResolver
{
    private const string CacheKeyPrefix = "Friday:Admin:EffectivePermissionKeys:v1:u:";

    public async Task<IReadOnlySet<string>> GetEffectivePermissionKeysAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        long generation = grantCacheCoordinator.CurrentGeneration;
        string cacheKey = CacheKeyPrefix + userId;

        if (
            memoryCache.TryGetValue(cacheKey, out CachedEffectiveGrants? cached)
            && cached is not null
            && cached.Generation == generation
        )
        {
            return cached.Keys;
        }

        List<string> keys = await dbContext
            .Set<UserRole>()
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => r)
            .Where(r => r.IsActive)
            .Join(dbContext.Set<RoleRight>(), r => r.Id, rr => rr.RoleId, (r, rr) => rr)
            .Join(dbContext.Set<Right>(), rr => rr.RightId, right => right.Id, (rr, right) => right)
            .Select(right => PermissionKeys.Format(right.Module, right.Resource, right.AccessLevel))
            .Distinct()
            .ToListAsync(cancellationToken);

        HashSet<string> set = keys.ToHashSet(StringComparer.Ordinal);
        int ttlMinutes = Math.Clamp(cacheOptions.Value.GrantListTtlMinutes, 1, 24 * 60);
        memoryCache.Set(
            cacheKey,
            new CachedEffectiveGrants(generation, set),
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ttlMinutes),
            }
        );

        return set;
    }

    private sealed record CachedEffectiveGrants(long Generation, HashSet<string> Keys);
}
