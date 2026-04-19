namespace Friday.Modules.Admin.Application.Models;

public sealed record RightDto(
    int Id,
    string Module,
    string Resource,
    string AccessLevel,
    string Name,
    string Description,
    string PermissionKey
);
