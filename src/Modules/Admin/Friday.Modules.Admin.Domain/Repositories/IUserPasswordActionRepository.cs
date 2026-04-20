using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;

namespace Friday.Modules.Admin.Domain.Repositories;

public interface IUserPasswordActionRepository
{
    Task AddAsync(UserPasswordAction action, CancellationToken cancellationToken = default);
    Task<UserPasswordAction?> GetActiveByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default
    );
    Task InvalidateActiveForUserAsync(
        int userId,
        string actionType,
        CancellationToken cancellationToken = default
    );
}
