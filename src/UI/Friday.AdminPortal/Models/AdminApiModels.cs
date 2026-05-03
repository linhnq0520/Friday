namespace Friday.AdminPortal.Models;

public sealed record ApiResponse<T>(string Code, string Message, T? Data, string? TraceId);

public sealed record LoginRequest(string Login, string Password);
public sealed record LogoutRequest(string RefreshToken);

public sealed record LoginResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken
);

public sealed record UserDto(
    int Id,
    string UserCode,
    string Username,
    string Email,
    string FullName,
    bool IsActive,
    bool IsLocked,
    int[] RoleIds
);

public sealed record RoleDto(int Id, string Code, string Name, bool IsActive, int[] RightIds);

public sealed record RightDto(
    int Id,
    string Module,
    string Resource,
    string AccessLevel,
    string Name,
    string Description,
    string PermissionKey
);

public sealed record CreateUserRequest(
    string UserCode,
    string Username,
    string Email,
    string FullName,
    string? Password,
    string? Phone,
    string? Address,
    string? CompanyName,
    string? JobTitle,
    string? Notes,
    int[] RoleIds
);

public sealed record CreateRoleRequest(string Code, string Name);
