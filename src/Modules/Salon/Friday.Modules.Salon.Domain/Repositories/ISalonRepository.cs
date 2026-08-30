using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Domain.Repositories;

public interface ISalonRepository
{
    Task<IReadOnlyList<HairService>> GetActiveServicesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HairService>> GetAllServicesAsync(CancellationToken cancellationToken = default);
    Task<HairService?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddServiceAsync(HairService service, CancellationToken cancellationToken = default);
    Task DeleteServiceAsync(HairService service, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Stylist>> GetActiveStylistsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stylist>> GetAllStylistsAsync(CancellationToken cancellationToken = default);
    Task<Stylist?> GetStylistByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddStylistAsync(Stylist stylist, CancellationToken cancellationToken = default);
    Task DeleteStylistAsync(Stylist stylist, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Partner>> GetActivePartnersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Partner>> GetAllPartnersAsync(CancellationToken cancellationToken = default);
    Task<Partner?> GetPartnerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddPartnerAsync(Partner partner, CancellationToken cancellationToken = default);
    Task DeletePartnerAsync(Partner partner, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShowcaseItem>> GetPublishedShowcaseAsync(
        ShowcaseType type,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyList<ShowcaseItem>> GetAllShowcaseAsync(
        ShowcaseType? type,
        CancellationToken cancellationToken = default
    );
    Task<ShowcaseItem?> GetShowcaseByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddShowcaseItemAsync(ShowcaseItem item, CancellationToken cancellationToken = default);
    Task DeleteShowcaseItemAsync(ShowcaseItem item, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GalleryItem>> GetPublishedGalleryAsync(
        GalleryCategory? category,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyList<GalleryItem>> GetAllGalleryAsync(CancellationToken cancellationToken = default);
    Task<GalleryItem?> GetGalleryByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddGalleryItemAsync(GalleryItem item, CancellationToken cancellationToken = default);
    Task DeleteGalleryItemAsync(GalleryItem item, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Promotion>> GetPublishedPromotionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Promotion>> GetAllPromotionsAsync(CancellationToken cancellationToken = default);
    Task<Promotion?> GetPromotionByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddPromotionAsync(Promotion promotion, CancellationToken cancellationToken = default);
    Task DeletePromotionAsync(Promotion promotion, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Testimonial>> GetPublishedTestimonialsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Testimonial>> GetAllTestimonialsAsync(CancellationToken cancellationToken = default);
    Task<Testimonial?> GetTestimonialByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddTestimonialAsync(Testimonial testimonial, CancellationToken cancellationToken = default);
    Task DeleteTestimonialAsync(Testimonial testimonial, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SiteSection>> GetVisibleSectionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SiteSection>> GetAllSectionsAsync(CancellationToken cancellationToken = default);
    Task<SiteSection?> GetSectionByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SiteSection?> GetSectionByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task AddSectionAsync(SiteSection section, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BeforeAfterItem>> GetPublishedBeforeAfterAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BeforeAfterItem>> GetAllBeforeAfterAsync(CancellationToken cancellationToken = default);
    Task<BeforeAfterItem?> GetBeforeAfterByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddBeforeAfterAsync(BeforeAfterItem item, CancellationToken cancellationToken = default);
    Task DeleteBeforeAfterAsync(BeforeAfterItem item, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> GetSettingsAsync(CancellationToken cancellationToken = default);
    Task UpsertSettingAsync(string key, string value, CancellationToken cancellationToken = default);

    Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<Appointment?> GetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetAppointmentsAsync(
        DateTime? from,
        DateTime? to,
        AppointmentStatus? status,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<AdminUser>> GetAllAdminUsersAsync(CancellationToken cancellationToken = default);
    Task<AdminUser?> GetAdminUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AdminUser?> GetAdminByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> AnyAdminUsersAsync(CancellationToken cancellationToken = default);
    Task AddAdminUserAsync(AdminUser user, CancellationToken cancellationToken = default);
    Task DeleteAdminUserAsync(AdminUser user, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlogPost>> GetPublishedBlogPostsAsync(
        string? category = null,
        int page = 1,
        int pageSize = 10,
        string? search = null,
        CancellationToken cancellationToken = default
    );
    Task<int> CountPublishedBlogPostsAsync(
        string? category = null,
        string? search = null,
        CancellationToken cancellationToken = default
    );
    Task<BlogPost?> GetBlogPostBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<BlogPost?> GetBlogPostByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlogPost>> GetAllBlogPostsAsync(
        string? category = null,
        string? search = null,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyList<BlogPost>> GetLatestBlogPostsAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlogPost>> GetFeaturedBlogPostsAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BlogPost>> GetRelatedBlogPostsAsync(
        int currentId,
        string category,
        int count = 3,
        CancellationToken cancellationToken = default
    );
    Task<IReadOnlyList<string>> GetDistinctCategoriesAsync(CancellationToken cancellationToken = default);
    Task AddBlogPostAsync(BlogPost post, CancellationToken cancellationToken = default);
    Task DeleteBlogPostAsync(BlogPost post, CancellationToken cancellationToken = default);
    Task IncrementBlogPostViewAsync(int id, CancellationToken cancellationToken = default);
}
