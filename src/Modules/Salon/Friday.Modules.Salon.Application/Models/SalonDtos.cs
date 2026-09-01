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
    DateTime? StartDate,
    DateTime? EndDate,
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

public sealed record ShowcaseItemDto(int Id, string Title, string ImageUrl);

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
    IReadOnlyList<ShowcaseItemDto> FeedbackShowcase,
    IReadOnlyList<ShowcaseItemDto> BeforeAfterShowcase,
    IReadOnlyList<StylistDto> Stylists,
    IReadOnlyList<PartnerDto> Partners,
    IReadOnlyList<BlogPostDto> BlogPosts
);

public sealed record BlogPostDto(
    int Id,
    string Title,
    string Slug,
    string? Summary,
    string? ThumbnailUrl,
    string? VideoUrl,
    string Category,
    string AuthorName,
    DateTime? PublishedAt,
    bool IsFeatured,
    int ViewCount
);

public sealed record BlogPostDetailDto(
    int Id,
    string Title,
    string Slug,
    string? Summary,
    string? Content,
    string? ThumbnailUrl,
    string? VideoUrl,
    string Category,
    string AuthorName,
    DateTime? PublishedAt,
    bool IsFeatured,
    int ViewCount,
    string? MetaTitle,
    string? MetaDescription,
    string? MetaKeywords,
    IReadOnlyList<BlogPostDto> RelatedPosts
);

public sealed record BlogListResultDto(
    IReadOnlyList<BlogPostDto> Items,
    IReadOnlyList<string> Categories,
    string? SelectedCategory,
    string? SearchQuery,
    int CurrentPage,
    int PageSize,
    int TotalItems,
    int TotalPages
);

public sealed record AdminUserDto(
    int Id,
    string Username,
    string DisplayName,
    AdminRole Role,
    bool IsActive,
    int? StylistId,
    DateTime CreatedOnUtc
);

public sealed record AdminLoginResult(
    bool Success,
    string? DisplayName,
    AdminRole? Role,
    string? ErrorMessage
);

public sealed record ChangePasswordResult(bool Success, string? ErrorMessage);

public sealed record CreateAdminUserResult(
    bool Success,
    int? Id,
    string? ErrorMessage
);

public sealed record UpdateAdminUserResult(bool Success, string? ErrorMessage);

public sealed record CreateAppointmentResult(
    bool Success,
    int? AppointmentId,
    string? ErrorMessage
);
