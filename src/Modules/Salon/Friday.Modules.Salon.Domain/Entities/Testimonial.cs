using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class Testimonial : Entity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; } = 5;
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;
}
