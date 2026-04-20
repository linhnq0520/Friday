using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Friday.Modules.Admin.Infrastructure.Repositories;

public sealed class UserPasswordActionRepository(FridayDbContext dbContext)
    : IUserPasswordActionRepository
{
    public async Task AddAsync(UserPasswordAction action, CancellationToken cancellationToken = default)
    {
        await dbContext.Set<UserPasswordAction>().AddAsync(action, cancellationToken);
    }

    public Task<UserPasswordAction?> GetActiveByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    )
    {
        DateTime now = DateTime.UtcNow;
        return dbContext.Set<UserPasswordAction>().FirstOrDefaultAsync(
            x => x.TokenHash == tokenHash && x.ConsumedAtUtc == null && x.ExpiresAtUtc > now,
            cancellationToken
        );
    }

    public async Task InvalidateActiveForUserAsync(
        int userId,
        string actionType,
        CancellationToken cancellationToken = default
    )
    {
        DateTime now = DateTime.UtcNow;
        string normalizedType = actionType.Trim().ToLowerInvariant();
        List<UserPasswordAction> active = await dbContext
            .Set<UserPasswordAction>()
            .Where(
                x =>
                    x.UserId == userId
                    && x.ActionType == normalizedType
                    && x.ConsumedAtUtc == null
                    && x.ExpiresAtUtc > now
            )
            .ToListAsync(cancellationToken);

        foreach (UserPasswordAction action in active)
        {
            action.Consume();
        }
    }
}
