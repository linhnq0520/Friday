using Friday.BuildingBlocks.Domain.Entities;
using Friday.Modules.Admin.Domain.Events;

namespace Friday.Modules.Admin.Domain.Aggregates.UserAggregate;

public sealed class User : AggregateRoot
{
    private readonly List<UserRole> _userRoles = [];

    private User() { }

    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public bool IsLocked { get; private set; }

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    public static User Create(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        User user = new()
        {
            Username = username.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            IsActive = true,
            IsLocked = false,
        };

        user.Raise(new UserCreatedDomainEvent(user.Id, user.Username, user.Email));
        return user;
    }

    public void AssignRole(int roleId)
    {
        if (roleId <= 0)
        {
            throw new ArgumentException("RoleId must be greater than zero.", nameof(roleId));
        }

        if (_userRoles.Any(x => x.RoleId == roleId))
        {
            return;
        }

        _userRoles.Add(UserRole.Create(Id, roleId));
        Touch();
        Raise(new UserRoleAssignedDomainEvent(Id, roleId));
    }

    public void Lock()
    {
        if (IsLocked)
        {
            return;
        }

        IsLocked = true;
        Touch();
        Raise(new UserLockedDomainEvent(Id));
    }

    public void Unlock()
    {
        if (!IsLocked)
        {
            return;
        }

        IsLocked = false;
        Touch();
    }
}
