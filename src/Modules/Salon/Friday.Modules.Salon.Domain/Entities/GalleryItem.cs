using Friday.BuildingBlocks.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class GalleryItem : Entity
{
    public string Title { get; set; } = string.Empty;
    public GalleryCategory Category { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;
}
