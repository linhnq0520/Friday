using Friday.BuildingBlocks.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class AdminUser : Entity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public AdminRole Role { get; set; } = AdminRole.Admin;
    public int? StylistId { get; set; }
    public bool IsActive { get; set; } = true;
}
