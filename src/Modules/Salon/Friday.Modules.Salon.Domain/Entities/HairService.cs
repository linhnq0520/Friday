using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class HairService : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PriceFrom { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int RatingDisplay { get; set; } = 5;
}
