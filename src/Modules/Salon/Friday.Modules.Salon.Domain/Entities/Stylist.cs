using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class Stylist : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
