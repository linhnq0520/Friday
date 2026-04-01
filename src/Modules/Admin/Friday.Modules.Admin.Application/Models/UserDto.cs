namespace Friday.Modules.Admin.Application.Models;

public sealed record UserDto(
    int Id,
    string Username,
    string Email,
    bool IsActive,
    bool IsLocked,
    int[] RoleIds
);
