using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class SiteSection : Entity
{
    public string SectionKey { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Body { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
}
