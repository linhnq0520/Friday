using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class HairService : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Headline { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public decimal PriceFrom { get; set; }
    public string? PriceTagText { get; set; }
    public string? DurationText { get; set; }
    public string? BadgeText { get; set; }
    public string? ImageUrl { get; set; }
    public string? HeroImageUrl { get; set; }
    public string? BeforeImageUrl { get; set; }
    public string? AfterImageUrl { get; set; }
    public string? PricingTableJson { get; set; }
    public string? MethodsJson { get; set; }
    public string? StepsJson { get; set; }
    public string? FaqsJson { get; set; }
    public string? ContentHtml { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int RatingDisplay { get; set; } = 5;
}
