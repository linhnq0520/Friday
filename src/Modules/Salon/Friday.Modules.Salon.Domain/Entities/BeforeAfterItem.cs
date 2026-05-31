using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class BeforeAfterItem : Entity
{
    public string Title { get; set; } = string.Empty;
    public string BeforeImageUrl { get; set; } = string.Empty;
    public string AfterImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;
}
