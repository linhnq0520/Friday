using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class Promotion : Entity
{
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; } = true;
}
