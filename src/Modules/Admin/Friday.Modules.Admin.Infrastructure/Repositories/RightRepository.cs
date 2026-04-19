using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Friday.Modules.Admin.Infrastructure.Repositories;

public sealed class RightRepository(FridayDbContext dbContext) : IRightRepository
{
    public async Task AddAsync(Right right, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<Right>().AddAsync(right, cancellationToken);
    }

    public Task<Right?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Set<Right>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(
        string module,
        string resource,
        PermissionAccessLevel accessLevel,
        CancellationToken cancellationToken = default
    )
    {
        string m = module.Trim().ToLowerInvariant();
        string r = resource.Trim().ToLowerInvariant();
        return dbContext.Set<Right>().AnyAsync(x => x.Module == m && x.Resource == r && x.AccessLevel == accessLevel, cancellationToken);
    }

    public async Task<IReadOnlyList<Right>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext
            .Set<Right>()
            .OrderBy(x => x.Module)
            .ThenBy(x => x.Resource)
            .ThenBy(x => x.AccessLevel)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Right>> GetByIdsAsync(
        int[] ids,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Set<Right>()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }
}
