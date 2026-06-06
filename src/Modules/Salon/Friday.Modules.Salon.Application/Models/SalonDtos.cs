using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Application.Models;

public sealed record HairServiceDto(
    int Id,
    string Name,
    string? Description,
    decimal PriceFrom,
    string? ImageUrl,
    int RatingDisplay
);

public sealed record StylistDto(int Id, string Name, string? Title, string? Bio, string? ImageUrl);

public sealed record PartnerDto(
    int Id,
    string Name,
    string? Description,
    string? LogoUrl,
    string? WebsiteUrl
);

public sealed record GalleryItemDto(
    int Id,
    string Title,
    GalleryCategory Category,
    string ImageUrl
);

public sealed record GalleryCollectionDto(
    GalleryCategory Category,
    string CoverImageUrl,
    int ImageCount
);

public sealed record PromotionDto(
    int Id,
    string Title,
    string? Summary,
    string? Content,
    string? ImageUrl,
    DateTime? PublishedAt
);

public sealed record TestimonialDto(
    int Id,
    string CustomerName,
    string Content,
    int Rating,
    string? ImageUrl
);

public sealed record SiteSectionDto(
    int Id,
    string SectionKey,
    string? Title,
    string? Subtitle,
    string? Body,
    string? ImageUrl
);

public sealed record BeforeAfterDto(
    int Id,
    string Title,
    string BeforeImageUrl,
    string AfterImageUrl
);

public sealed record AppointmentDto(
    int Id,
    string CustomerName,
    string Phone,
    string? Email,
    int HairServiceId,
    string? ServiceName,
    int? StylistId,
    string? StylistName,
    DateTime ScheduledAt,
    string? Notes,
    AppointmentStatus Status
);

public sealed record HomePageDto(
    IReadOnlyDictionary<string, string> Settings,
    IReadOnlyList<SiteSectionDto> Sections,
    IReadOnlyList<GalleryCollectionDto> GalleryCollections,
    IReadOnlyList<HairServiceDto> Services,
    IReadOnlyList<PromotionDto> Promotions,
    IReadOnlyList<TestimonialDto> Testimonials,
    IReadOnlyList<BeforeAfterDto> BeforeAfter,
    IReadOnlyList<StylistDto> Stylists,
    IReadOnlyList<PartnerDto> Partners
);

public sealed record AdminLoginResult(bool Success, string? DisplayName, string? ErrorMessage);

public sealed record CreateAppointmentResult(
    bool Success,
    int? AppointmentId,
    string? ErrorMessage
);
