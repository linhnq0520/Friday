using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;

namespace Friday.Modules.Salon.Application.Features;

public sealed record GetHomePageQuery : IQuery<HomePageDto>;

public sealed class GetHomePageHandler(ISalonRepository repository)
    : IQueryHandler<GetHomePageQuery, HomePageDto>
{
    public async Task<HomePageDto> HandleAsync(
        GetHomePageQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        IReadOnlyList<SiteSection> sections = await repository.GetVisibleSectionsAsync(
            cancellationToken
        );
        IReadOnlyList<GalleryItem> gallery = await repository.GetPublishedGalleryAsync(
            null,
            cancellationToken
        );
        IReadOnlyList<HairService> services = await repository.GetActiveServicesAsync(
            cancellationToken
        );
        IReadOnlyList<Promotion> promotions = await repository.GetPublishedPromotionsAsync(
            cancellationToken
        );
        IReadOnlyList<Testimonial> testimonials = await repository.GetPublishedTestimonialsAsync(
            cancellationToken
        );
        IReadOnlyList<ShowcaseItem> feedbackShowcase = await repository.GetPublishedShowcaseAsync(
            ShowcaseType.Feedback,
            cancellationToken
        );
        IReadOnlyList<ShowcaseItem> beforeAfterShowcase = await repository.GetPublishedShowcaseAsync(
            ShowcaseType.BeforeAfter,
            cancellationToken
        );
        IReadOnlyList<Stylist> stylists = await repository.GetActiveStylistsAsync(
            cancellationToken
        );
        IReadOnlyList<Partner> partners = await repository.GetActivePartnersAsync(
            cancellationToken
        );

        IReadOnlyList<BlogPost> blogPosts = await repository.GetLatestBlogPostsAsync(
            4,
            cancellationToken
        );

        return new HomePageDto(
            settings,
            sections.Select(MapSection).ToList(),
            BuildGalleryCollections(gallery),
            services.Select(MapService).ToList(),
            promotions.Take(6).Select(MapPromotion).ToList(),
            testimonials.Take(6).Select(MapTestimonial).ToList(),
            feedbackShowcase.Select(MapShowcase).ToList(),
            beforeAfterShowcase.Select(MapShowcase).ToList(),
            stylists.Select(MapStylist).ToList(),
            partners.Select(MapPartner).ToList(),
            blogPosts.Select(MapBlogPost).ToList()
        );
    }

    internal static SiteSectionDto MapSection(SiteSection x) =>
        new(x.Id, x.SectionKey, x.Title, x.Subtitle, x.Body, x.ImageUrl);

    internal static HairServiceDto MapService(HairService x) =>
        new(x.Id, x.Name, x.Description, x.PriceFrom, x.ImageUrl, x.RatingDisplay);

    internal static StylistDto MapStylist(Stylist x) =>
        new(x.Id, x.Name, x.Title, x.Bio, x.ImageUrl);

    internal static PartnerDto MapPartner(Partner x) =>
        new(x.Id, x.Name, x.Description, x.LogoUrl, x.WebsiteUrl);

    internal static GalleryItemDto MapGallery(GalleryItem x) =>
        new(x.Id, x.Title, x.Category, x.ImageUrl);

    internal static IReadOnlyList<GalleryCollectionDto> BuildGalleryCollections(
        IReadOnlyList<GalleryItem> items
    ) =>
        GalleryCategoryInfo.CollectionCategories
            .Select(category =>
            {
                IReadOnlyList<GalleryItem> categoryItems = items
                    .Where(x => x.Category == category)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                string cover =
                    categoryItems.FirstOrDefault()?.ImageUrl
                    ?? GalleryCategoryInfo.GetDefaultCover(category);

                return new GalleryCollectionDto(category, cover, categoryItems.Count);
            })
            .ToList();

    internal static PromotionDto MapPromotion(Promotion x) =>
        new(x.Id, x.Title, x.Summary, x.Content, x.ImageUrl, x.StartDate, x.EndDate, x.PublishedAt);

    internal static TestimonialDto MapTestimonial(Testimonial x) =>
        new(x.Id, x.CustomerName, x.Content, x.Rating, x.ImageUrl);

    internal static BeforeAfterDto MapBeforeAfter(BeforeAfterItem x) =>
        new(x.Id, x.Title, x.BeforeImageUrl, x.AfterImageUrl);

    internal static ShowcaseItemDto MapShowcase(ShowcaseItem x) =>
        new(x.Id, x.Title, x.ImageUrl);

    internal static BlogPostDto MapBlogPost(BlogPost x) =>
        new(
            x.Id,
            x.Title,
            x.Slug,
            x.Summary,
            x.ThumbnailUrl,
            x.VideoUrl,
            x.Category,
            x.AuthorName,
            x.PublishedAt,
            x.IsFeatured,
            x.ViewCount
        );

    internal static AppointmentDto MapAppointment(Appointment x) =>
        new(
            x.Id,
            x.CustomerName,
            x.Phone,
            x.Email,
            x.HairServiceId,
            x.HairService?.Name,
            x.StylistId,
            x.Stylist?.Name,
            x.ScheduledAt,
            x.Notes,
            x.Status
        );
}

public sealed record GetServicesPageQuery : IQuery<IReadOnlyList<HairServiceDto>>;

public sealed class GetServicesPageHandler(ISalonRepository repository)
    : IQueryHandler<GetServicesPageQuery, IReadOnlyList<HairServiceDto>>
{
    public async Task<IReadOnlyList<HairServiceDto>> HandleAsync(
        GetServicesPageQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<HairService> services = await repository.GetActiveServicesAsync(
            cancellationToken
        );
        return services.Select(GetHomePageHandler.MapService).ToList();
    }
}

public sealed record GetGalleryCollectionsQuery : IQuery<IReadOnlyList<GalleryCollectionDto>>;

public sealed class GetGalleryCollectionsHandler(ISalonRepository repository)
    : IQueryHandler<GetGalleryCollectionsQuery, IReadOnlyList<GalleryCollectionDto>>
{
    public async Task<IReadOnlyList<GalleryCollectionDto>> HandleAsync(
        GetGalleryCollectionsQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<GalleryItem> items = await repository.GetPublishedGalleryAsync(
            null,
            cancellationToken
        );
        return GetHomePageHandler.BuildGalleryCollections(items);
    }
}

public sealed record GetGalleryPageQuery(GalleryCategory? Category)
    : IQuery<IReadOnlyList<GalleryItemDto>>;

public sealed class GetGalleryPageHandler(ISalonRepository repository)
    : IQueryHandler<GetGalleryPageQuery, IReadOnlyList<GalleryItemDto>>
{
    public async Task<IReadOnlyList<GalleryItemDto>> HandleAsync(
        GetGalleryPageQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<GalleryItem> items = await repository.GetPublishedGalleryAsync(
            request.Category,
            cancellationToken
        );
        return items.Select(GetHomePageHandler.MapGallery).ToList();
    }
}

public sealed record GetPromotionsPageQuery : IQuery<IReadOnlyList<PromotionDto>>;

public sealed class GetPromotionsPageHandler(ISalonRepository repository)
    : IQueryHandler<GetPromotionsPageQuery, IReadOnlyList<PromotionDto>>
{
    public async Task<IReadOnlyList<PromotionDto>> HandleAsync(
        GetPromotionsPageQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Promotion> items = await repository.GetPublishedPromotionsAsync(
            cancellationToken
        );
        return items.Select(GetHomePageHandler.MapPromotion).ToList();
    }
}

public sealed record GetBookingFormQuery : IQuery<BookingFormDto>;

public sealed record BookingFormDto(
    IReadOnlyList<HairServiceDto> Services,
    IReadOnlyList<StylistDto> Stylists
);

public sealed class GetBookingFormHandler(ISalonRepository repository)
    : IQueryHandler<GetBookingFormQuery, BookingFormDto>
{
    public async Task<BookingFormDto> HandleAsync(
        GetBookingFormQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<HairService> services = await repository.GetActiveServicesAsync(
            cancellationToken
        );
        IReadOnlyList<Stylist> stylists = await repository.GetActiveStylistsAsync(
            cancellationToken
        );
        return new BookingFormDto(
            services.Select(GetHomePageHandler.MapService).ToList(),
            stylists.Select(GetHomePageHandler.MapStylist).ToList()
        );
    }
}

public sealed record CreateAppointmentCommand(
    string CustomerName,
    string Phone,
    string? Email,
    int HairServiceId,
    int? StylistId,
    DateTime ScheduledAt,
    string? Notes
) : ICommand<CreateAppointmentResult>;

public sealed class CreateAppointmentHandler(ISalonRepository repository)
    : ICommandHandler<CreateAppointmentCommand, CreateAppointmentResult>
{
    public async Task<CreateAppointmentResult> HandleAsync(
        CreateAppointmentCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            return new CreateAppointmentResult(false, null, "Vui lòng nhập họ tên.");
        }

        if (string.IsNullOrWhiteSpace(request.Phone))
        {
            return new CreateAppointmentResult(false, null, "Vui lòng nhập số điện thoại.");
        }

        HairService? service = await repository.GetServiceByIdAsync(
            request.HairServiceId,
            cancellationToken
        );
        if (service is null || !service.IsActive)
        {
            return new CreateAppointmentResult(false, null, "Dịch vụ không hợp lệ.");
        }

        if (request.StylistId.HasValue)
        {
            Stylist? stylist = await repository.GetStylistByIdAsync(
                request.StylistId.Value,
                cancellationToken
            );
            if (stylist is null || !stylist.IsActive)
            {
                return new CreateAppointmentResult(false, null, "Thợ không hợp lệ.");
            }
        }

        if (request.ScheduledAt <= DateTime.Now)
        {
            return new CreateAppointmentResult(
                false,
                null,
                "Vui lòng chọn thời gian trong tương lai."
            );
        }

        Appointment appointment = new()
        {
            CustomerName = request.CustomerName.Trim(),
            Phone = request.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            HairServiceId = request.HairServiceId,
            StylistId = request.StylistId,
            ScheduledAt = request.ScheduledAt,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Status = AppointmentStatus.Pending,
        };

        await repository.AddAppointmentAsync(appointment, cancellationToken);
        return new CreateAppointmentResult(true, appointment.Id, null);
    }
}

public sealed record AdminLoginCommand(string Username, string Password) : IQuery<AdminLoginResult>;

public sealed class AdminLoginHandler(
    ISalonRepository repository,
    Security.IAdminPasswordService passwordService
) : IQueryHandler<AdminLoginCommand, AdminLoginResult>
{
    public async Task<AdminLoginResult> HandleAsync(
        AdminLoginCommand request,
        CancellationToken cancellationToken
    )
    {
        AdminUser? user = await repository.GetAdminByUsernameAsync(
            request.Username,
            cancellationToken
        );
        if (user is null || !passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return new AdminLoginResult(false, null, null, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        if (!user.IsActive)
        {
            return new AdminLoginResult(false, null, null, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
        }

        return new AdminLoginResult(true, user.DisplayName, user.Role, null);
    }
}

public sealed record ChangePasswordCommand(
    string Username,
    string CurrentPassword,
    string NewPassword
) : IQuery<ChangePasswordResult>;

public sealed class ChangePasswordHandler(
    ISalonRepository repository,
    Security.IAdminPasswordService passwordService
) : IQueryHandler<ChangePasswordCommand, ChangePasswordResult>
{
    public async Task<ChangePasswordResult> HandleAsync(
        ChangePasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        AdminUser? user = await repository.GetAdminByUsernameAsync(request.Username, cancellationToken);
        if (user is null)
        {
            return new ChangePasswordResult(false, "Không tìm thấy thông tin tài khoản.");
        }

        if (!passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return new ChangePasswordResult(false, "Mật khẩu hiện tại không chính xác.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            return new ChangePasswordResult(false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
        }

        user.PasswordHash = passwordService.HashPassword(request.NewPassword);
        await repository.AddAdminUserAsync(user, cancellationToken);
        return new ChangePasswordResult(true, null);
    }
}

public sealed record GetAllAdminUsersQuery : IQuery<IReadOnlyList<AdminUserDto>>;

public sealed class GetAllAdminUsersHandler(ISalonRepository repository)
    : IQueryHandler<GetAllAdminUsersQuery, IReadOnlyList<AdminUserDto>>
{
    public async Task<IReadOnlyList<AdminUserDto>> HandleAsync(
        GetAllAdminUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<AdminUser> users = await repository.GetAllAdminUsersAsync(cancellationToken);
        return users.Select(u => new AdminUserDto(
            u.Id,
            u.Username,
            u.DisplayName,
            u.Role,
            u.IsActive,
            u.StylistId,
            u.CreatedOnUtc
        )).ToList();
    }
}

public sealed record GetAdminUserByIdQuery(int Id) : IQuery<AdminUserDto?>;

public sealed class GetAdminUserByIdHandler(ISalonRepository repository)
    : IQueryHandler<GetAdminUserByIdQuery, AdminUserDto?>
{
    public async Task<AdminUserDto?> HandleAsync(
        GetAdminUserByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        AdminUser? u = await repository.GetAdminUserByIdAsync(request.Id, cancellationToken);
        if (u is null) return null;
        return new AdminUserDto(
            u.Id,
            u.Username,
            u.DisplayName,
            u.Role,
            u.IsActive,
            u.StylistId,
            u.CreatedOnUtc
        );
    }
}

public sealed record CreateAdminUserCommand(
    string Username,
    string DisplayName,
    string Password,
    AdminRole Role,
    int? StylistId
) : IQuery<CreateAdminUserResult>;

public sealed class CreateAdminUserHandler(
    ISalonRepository repository,
    Security.IAdminPasswordService passwordService
) : IQueryHandler<CreateAdminUserCommand, CreateAdminUserResult>
{
    public async Task<CreateAdminUserResult> HandleAsync(
        CreateAdminUserCommand request,
        CancellationToken cancellationToken
    )
    {
        string username = request.Username.Trim();
        if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
        {
            return new CreateAdminUserResult(false, null, "Tên đăng nhập phải có ít nhất 3 ký tự.");
        }

        AdminUser? existing = await repository.GetAdminByUsernameAsync(username, cancellationToken);
        if (existing is not null)
        {
            return new CreateAdminUserResult(false, null, "Tên đăng nhập này đã tồn tại trong hệ thống.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
        {
            return new CreateAdminUserResult(false, null, "Mật khẩu phải có ít nhất 6 ký tự.");
        }

        AdminUser user = new()
        {
            Username = username,
            DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? username : request.DisplayName.Trim(),
            PasswordHash = passwordService.HashPassword(request.Password),
            Role = request.Role,
            StylistId = request.StylistId,
            IsActive = true,
        };

        await repository.AddAdminUserAsync(user, cancellationToken);
        return new CreateAdminUserResult(true, user.Id, null);
    }
}

public sealed record UpdateAdminUserCommand(
    int Id,
    string DisplayName,
    AdminRole Role,
    bool IsActive,
    int? StylistId,
    string? NewPassword
) : IQuery<UpdateAdminUserResult>;

public sealed class UpdateAdminUserHandler(
    ISalonRepository repository,
    Security.IAdminPasswordService passwordService
) : IQueryHandler<UpdateAdminUserCommand, UpdateAdminUserResult>
{
    public async Task<UpdateAdminUserResult> HandleAsync(
        UpdateAdminUserCommand request,
        CancellationToken cancellationToken
    )
    {
        AdminUser? user = await repository.GetAdminUserByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return new UpdateAdminUserResult(false, "Không tìm thấy người dùng.");
        }

        user.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? user.Username : request.DisplayName.Trim();
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.StylistId = request.StylistId;

        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (request.NewPassword.Length < 6)
            {
                return new UpdateAdminUserResult(false, "Mật khẩu mới phải có ít nhất 6 ký tự.");
            }
            user.PasswordHash = passwordService.HashPassword(request.NewPassword);
        }

        await repository.AddAdminUserAsync(user, cancellationToken);
        return new UpdateAdminUserResult(true, null);
    }
}

public sealed record DeleteAdminUserCommand(int Id, string CurrentAdminUsername) : IQuery<UpdateAdminUserResult>;

public sealed class DeleteAdminUserHandler(ISalonRepository repository)
    : IQueryHandler<DeleteAdminUserCommand, UpdateAdminUserResult>
{
    public async Task<UpdateAdminUserResult> HandleAsync(
        DeleteAdminUserCommand request,
        CancellationToken cancellationToken
    )
    {
        AdminUser? user = await repository.GetAdminUserByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return new UpdateAdminUserResult(false, "Không tìm thấy người dùng.");
        }

        if (string.Equals(user.Username, request.CurrentAdminUsername, StringComparison.OrdinalIgnoreCase))
        {
            return new UpdateAdminUserResult(false, "Bạn không thể tự xóa tài khoản đang đăng nhập của chính mình.");
        }

        IReadOnlyList<AdminUser> allUsers = await repository.GetAllAdminUsersAsync(cancellationToken);
        int adminCount = allUsers.Count(x => x.Role == AdminRole.Admin && x.IsActive);
        if (user.Role == AdminRole.Admin && adminCount <= 1)
        {
            return new UpdateAdminUserResult(false, "Không thể xóa tài khoản Quản trị viên duy nhất còn lại.");
        }

        await repository.DeleteAdminUserAsync(user, cancellationToken);
        return new UpdateAdminUserResult(true, null);
    }
}

public sealed record GetAdminAppointmentsQuery(
    DateTime? From,
    DateTime? To,
    AppointmentStatus? Status
) : IQuery<IReadOnlyList<AppointmentDto>>;

public sealed class GetAdminAppointmentsHandler(ISalonRepository repository)
    : IQueryHandler<GetAdminAppointmentsQuery, IReadOnlyList<AppointmentDto>>
{
    public async Task<IReadOnlyList<AppointmentDto>> HandleAsync(
        GetAdminAppointmentsQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Appointment> items = await repository.GetAppointmentsAsync(
            request.From,
            request.To,
            request.Status,
            cancellationToken
        );
        return items.Select(GetHomePageHandler.MapAppointment).ToList();
    }
}

public sealed record UpdateAppointmentStatusCommand(int Id, AppointmentStatus Status)
    : ICommand<CreateAppointmentResult>;

public sealed class UpdateAppointmentStatusHandler(ISalonRepository repository)
    : ICommandHandler<UpdateAppointmentStatusCommand, CreateAppointmentResult>
{
    public async Task<CreateAppointmentResult> HandleAsync(
        UpdateAppointmentStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        Appointment? appointment = await repository.GetAppointmentByIdAsync(
            request.Id,
            cancellationToken
        );
        if (appointment is null)
        {
            return new CreateAppointmentResult(false, null, "Không tìm thấy lịch hẹn.");
        }

        appointment.Status = request.Status;
        appointment.Touch();
        await repository.AddAppointmentAsync(appointment, cancellationToken);
        return new CreateAppointmentResult(true, appointment.Id, null);
    }
}

public sealed record GetBlogPostsPageQuery(
    string? Category = null,
    int Page = 1,
    int PageSize = 9,
    string? Search = null
) : IQuery<BlogListResultDto>;

public sealed class GetBlogPostsPageHandler(ISalonRepository repository)
    : IQueryHandler<GetBlogPostsPageQuery, BlogListResultDto>
{
    public async Task<BlogListResultDto> HandleAsync(
        GetBlogPostsPageQuery request,
        CancellationToken cancellationToken
    )
    {
        int page = Math.Max(1, request.Page);
        int pageSize = Math.Max(1, Math.Min(50, request.PageSize));

        IReadOnlyList<BlogPost> items = await repository.GetPublishedBlogPostsAsync(
            request.Category,
            page,
            pageSize,
            request.Search,
            cancellationToken
        );

        int totalItems = await repository.CountPublishedBlogPostsAsync(
            request.Category,
            request.Search,
            cancellationToken
        );

        IReadOnlyList<string> categories = await repository.GetDistinctCategoriesAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        return new BlogListResultDto(
            items.Select(GetHomePageHandler.MapBlogPost).ToList(),
            categories,
            request.Category,
            request.Search,
            page,
            pageSize,
            totalItems,
            totalPages
        );
    }
}

public sealed record GetBlogPostDetailQuery(string Slug) : IQuery<BlogPostDetailDto?>;

public sealed class GetBlogPostDetailHandler(ISalonRepository repository)
    : IQueryHandler<GetBlogPostDetailQuery, BlogPostDetailDto?>
{
    public async Task<BlogPostDetailDto?> HandleAsync(
        GetBlogPostDetailQuery request,
        CancellationToken cancellationToken
    )
    {
        BlogPost? post = await repository.GetBlogPostBySlugAsync(request.Slug, cancellationToken);
        if (post is null || !post.IsPublished || (post.PublishedAt.HasValue && post.PublishedAt.Value > DateTime.UtcNow))
        {
            return null;
        }

        IReadOnlyList<BlogPost> related = await repository.GetRelatedBlogPostsAsync(
            post.Id,
            post.Category,
            3,
            cancellationToken
        );

        return new BlogPostDetailDto(
            post.Id,
            post.Title,
            post.Slug,
            post.Summary,
            post.Content,
            post.ThumbnailUrl,
            post.VideoUrl,
            post.Category,
            post.AuthorName,
            post.PublishedAt,
            post.IsFeatured,
            post.ViewCount,
            post.MetaTitle,
            post.MetaDescription,
            post.MetaKeywords,
            related.Select(GetHomePageHandler.MapBlogPost).ToList()
        );
    }
}

public sealed record GetLatestBlogPostsQuery(int Count) : IQuery<IReadOnlyList<BlogPostDto>>;

public sealed class GetLatestBlogPostsHandler(ISalonRepository repository)
    : IQueryHandler<GetLatestBlogPostsQuery, IReadOnlyList<BlogPostDto>>
{
    public async Task<IReadOnlyList<BlogPostDto>> HandleAsync(
        GetLatestBlogPostsQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<BlogPost> posts = await repository.GetLatestBlogPostsAsync(
            request.Count,
            cancellationToken
        );
        return posts.Select(GetHomePageHandler.MapBlogPost).ToList();
    }
}
