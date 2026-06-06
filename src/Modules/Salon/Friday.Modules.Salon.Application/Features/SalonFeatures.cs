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
            partners.Select(MapPartner).ToList()
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
        new(x.Id, x.Title, x.Summary, x.Content, x.ImageUrl, x.PublishedAt);

    internal static TestimonialDto MapTestimonial(Testimonial x) =>
        new(x.Id, x.CustomerName, x.Content, x.Rating, x.ImageUrl);

    internal static BeforeAfterDto MapBeforeAfter(BeforeAfterItem x) =>
        new(x.Id, x.Title, x.BeforeImageUrl, x.AfterImageUrl);

    internal static ShowcaseItemDto MapShowcase(ShowcaseItem x) =>
        new(x.Id, x.Title, x.ImageUrl);

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
            return new AdminLoginResult(false, null, "Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        return new AdminLoginResult(true, user.DisplayName, null);
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
