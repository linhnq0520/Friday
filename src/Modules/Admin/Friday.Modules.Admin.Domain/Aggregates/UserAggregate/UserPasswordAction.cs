namespace Friday.Modules.Admin.Domain.Aggregates.UserAggregate;

public sealed class UserPasswordAction
{
    private UserPasswordAction() { }

    public Guid Id { get; private set; }
    public int UserId { get; private set; }
    public string ActionType { get; private set; } = string.Empty;
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ConsumedAtUtc { get; private set; }

    public static UserPasswordAction Create(
        int userId,
        string actionType,
        string tokenHash,
        DateTime expiresAtUtc
    )
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(userId, 0);
        ArgumentException.ThrowIfNullOrWhiteSpace(actionType);
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        return new UserPasswordAction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionType = actionType.Trim().ToLowerInvariant(),
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
            CreatedOnUtc = DateTime.UtcNow,
        };
    }

    public bool IsActive(DateTime nowUtc) => ConsumedAtUtc is null && ExpiresAtUtc > nowUtc;

    public void Consume()
    {
        if (ConsumedAtUtc is null)
        {
            ConsumedAtUtc = DateTime.UtcNow;
        }
    }
}
