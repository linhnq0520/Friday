using Friday.BuildingBlocks.Domain.Entities;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using Friday.Modules.Salon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Friday.Modules.Salon.Infrastructure.Repositories;

public sealed class SalonRepository(SalonDbContext dbContext) : ISalonRepository
{
    public async Task<IReadOnlyList<HairService>> GetActiveServicesAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<HairService>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<HairService>> GetAllServicesAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<HairService>()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public Task<HairService?> GetServiceByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<HairService>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddServiceAsync(HairService service, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            service,
            static (source, target) =>
            {
                target.Name = source.Name;
                target.Description = source.Description;
                target.PriceFrom = source.PriceFrom;
                target.ImageUrl = source.ImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsActive = source.IsActive;
                target.RatingDisplay = source.RatingDisplay;
            },
            cancellationToken
        );

    public Task DeleteServiceAsync(HairService service, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<HairService>(service.Id, cancellationToken);

    public async Task<IReadOnlyList<Stylist>> GetActiveStylistsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<Stylist>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Stylist>> GetAllStylistsAsync(
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<Stylist>().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public Task<Stylist?> GetStylistByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Stylist>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddStylistAsync(Stylist stylist, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            stylist,
            static (source, target) =>
            {
                target.Name = source.Name;
                target.Title = source.Title;
                target.Bio = source.Bio;
                target.ImageUrl = source.ImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsActive = source.IsActive;
            },
            cancellationToken
        );

    public Task DeleteStylistAsync(Stylist stylist, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<Stylist>(stylist.Id, cancellationToken);

    public async Task<IReadOnlyList<Partner>> GetActivePartnersAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<Partner>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Partner>> GetAllPartnersAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<Partner>()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public Task<Partner?> GetPartnerByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Partner>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddPartnerAsync(Partner partner, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            partner,
            static (source, target) =>
            {
                target.Name = source.Name;
                target.Description = source.Description;
                target.LogoUrl = source.LogoUrl;
                target.WebsiteUrl = source.WebsiteUrl;
                target.SortOrder = source.SortOrder;
                target.IsActive = source.IsActive;
            },
            cancellationToken
        );

    public Task DeletePartnerAsync(Partner partner, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<Partner>(partner.Id, cancellationToken);

    public async Task<IReadOnlyList<ShowcaseItem>> GetPublishedShowcaseAsync(
        ShowcaseType type,
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<ShowcaseItem>()
            .Where(x => x.IsPublished && x.Type == type)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ShowcaseItem>> GetAllShowcaseAsync(
        ShowcaseType? type,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<ShowcaseItem> query = dbContext.Set<ShowcaseItem>();

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        return await query
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<ShowcaseItem?> GetShowcaseByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<ShowcaseItem>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddShowcaseItemAsync(ShowcaseItem item, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            item,
            static (source, target) =>
            {
                target.Type = source.Type;
                target.Title = source.Title;
                target.ImageUrl = source.ImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsPublished = source.IsPublished;
            },
            cancellationToken
        );

    public Task DeleteShowcaseItemAsync(ShowcaseItem item, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<ShowcaseItem>(item.Id, cancellationToken);

    public async Task<IReadOnlyList<GalleryItem>> GetPublishedGalleryAsync(
        GalleryCategory? category,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<GalleryItem> query = dbContext
            .Set<GalleryItem>()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.SortOrder);

        if (category.HasValue)
        {
            query = query.Where(x => x.Category == category.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GalleryItem>> GetAllGalleryAsync(
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<GalleryItem>().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public Task<GalleryItem?> GetGalleryByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<GalleryItem>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddGalleryItemAsync(GalleryItem item, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            item,
            static (source, target) =>
            {
                target.Title = source.Title;
                target.Category = source.Category;
                target.ImageUrl = source.ImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsPublished = source.IsPublished;
            },
            cancellationToken
        );

    public Task DeleteGalleryItemAsync(GalleryItem item, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<GalleryItem>(item.Id, cancellationToken);

    public async Task<IReadOnlyList<Promotion>> GetPublishedPromotionsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<Promotion>()
            .Where(x => x.IsPublished)
            .OrderByDescending(x => x.PublishedAt)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Promotion>> GetAllPromotionsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<Promotion>()
            .OrderByDescending(x => x.PublishedAt)
            .ToListAsync(cancellationToken);

    public Task<Promotion?> GetPromotionByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Promotion>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddPromotionAsync(Promotion promotion, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            promotion,
            static (source, target) =>
            {
                target.Title = source.Title;
                target.Summary = source.Summary;
                target.Content = source.Content;
                target.ImageUrl = source.ImageUrl;
                target.PublishedAt = source.PublishedAt;
                target.IsPublished = source.IsPublished;
            },
            cancellationToken
        );

    public Task DeletePromotionAsync(Promotion promotion, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<Promotion>(promotion.Id, cancellationToken);

    public async Task<IReadOnlyList<Testimonial>> GetPublishedTestimonialsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<Testimonial>()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Testimonial>> GetAllTestimonialsAsync(
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<Testimonial>().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public Task<Testimonial?> GetTestimonialByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<Testimonial>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddTestimonialAsync(Testimonial testimonial, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            testimonial,
            static (source, target) =>
            {
                target.CustomerName = source.CustomerName;
                target.Content = source.Content;
                target.Rating = source.Rating;
                target.ImageUrl = source.ImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsPublished = source.IsPublished;
            },
            cancellationToken
        );

    public Task DeleteTestimonialAsync(Testimonial testimonial, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<Testimonial>(testimonial.Id, cancellationToken);

    public async Task<IReadOnlyList<SiteSection>> GetVisibleSectionsAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<SiteSection>()
            .Where(x => x.IsVisible)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SiteSection>> GetAllSectionsAsync(
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<SiteSection>().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public Task<SiteSection?> GetSectionByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<SiteSection>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<SiteSection?> GetSectionByKeyAsync(string key, CancellationToken cancellationToken = default) =>
        dbContext.Set<SiteSection>().FirstOrDefaultAsync(x => x.SectionKey == key, cancellationToken);

    public Task AddSectionAsync(SiteSection section, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            section,
            static (source, target) =>
            {
                target.SectionKey = source.SectionKey;
                target.Title = source.Title;
                target.Subtitle = source.Subtitle;
                target.Body = source.Body;
                target.ImageUrl = source.ImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsVisible = source.IsVisible;
            },
            cancellationToken
        );

    public async Task<IReadOnlyList<BeforeAfterItem>> GetPublishedBeforeAfterAsync(
        CancellationToken cancellationToken = default
    ) =>
        await dbContext
            .Set<BeforeAfterItem>()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<BeforeAfterItem>> GetAllBeforeAfterAsync(
        CancellationToken cancellationToken = default
    ) => await dbContext.Set<BeforeAfterItem>().OrderBy(x => x.SortOrder).ToListAsync(cancellationToken);

    public Task<BeforeAfterItem?> GetBeforeAfterByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Set<BeforeAfterItem>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task AddBeforeAfterAsync(BeforeAfterItem item, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            item,
            static (source, target) =>
            {
                target.Title = source.Title;
                target.BeforeImageUrl = source.BeforeImageUrl;
                target.AfterImageUrl = source.AfterImageUrl;
                target.SortOrder = source.SortOrder;
                target.IsPublished = source.IsPublished;
            },
            cancellationToken
        );

    public Task DeleteBeforeAfterAsync(BeforeAfterItem item, CancellationToken cancellationToken = default) =>
        DeleteByIdAsync<BeforeAfterItem>(item.Id, cancellationToken);

    public async Task<IReadOnlyDictionary<string, string>> GetSettingsAsync(
        CancellationToken cancellationToken = default
    )
    {
        List<SiteSetting> settings = await dbContext.Set<SiteSetting>().ToListAsync(cancellationToken);
        return settings.ToDictionary(x => x.SettingKey, x => x.Value, StringComparer.OrdinalIgnoreCase);
    }

    public async Task UpsertSettingAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        SiteSetting? existing = await dbContext
            .Set<SiteSetting>()
            .FirstOrDefaultAsync(x => x.SettingKey == key, cancellationToken);

        if (existing is null)
        {
            await dbContext
                .Set<SiteSetting>()
                .AddAsync(new SiteSetting { SettingKey = key, Value = value }, cancellationToken);
        }
        else
        {
            existing.Value = value;
            existing.Touch();
        }
    }

    public Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            appointment,
            static (source, target) =>
            {
                target.CustomerName = source.CustomerName;
                target.Phone = source.Phone;
                target.Email = source.Email;
                target.HairServiceId = source.HairServiceId;
                target.StylistId = source.StylistId;
                target.ScheduledAt = source.ScheduledAt;
                target.Notes = source.Notes;
                target.Status = source.Status;
            },
            cancellationToken
        );

    public Task<Appointment?> GetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext
            .Set<Appointment>()
            .Include(x => x.HairService)
            .Include(x => x.Stylist)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Appointment>> GetAppointmentsAsync(
        DateTime? from,
        DateTime? to,
        AppointmentStatus? status,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Appointment> query = dbContext
            .Set<Appointment>()
            .Include(x => x.HairService)
            .Include(x => x.Stylist)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.ScheduledAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.ScheduledAt <= to.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.OrderBy(x => x.ScheduledAt).ToListAsync(cancellationToken);
    }

    public Task<AdminUser?> GetAdminByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        string normalized = username.Trim().ToUpperInvariant();
        return dbContext
            .Set<AdminUser>()
            .FirstOrDefaultAsync(x => x.Username.ToUpper() == normalized && x.IsActive, cancellationToken);
    }

    public Task<bool> AnyAdminUsersAsync(CancellationToken cancellationToken = default) =>
        dbContext.Set<AdminUser>().AnyAsync(cancellationToken);

    public Task AddAdminUserAsync(AdminUser user, CancellationToken cancellationToken = default) =>
        UpsertAsync(
            user,
            static (source, target) =>
            {
                target.Username = source.Username;
                target.PasswordHash = source.PasswordHash;
                target.DisplayName = source.DisplayName;
                target.IsActive = source.IsActive;
            },
            cancellationToken
        );

    private async Task UpsertAsync<TEntity>(
        TEntity incoming,
        Action<TEntity, TEntity> applyValues,
        CancellationToken cancellationToken
    )
        where TEntity : Entity
    {
        if (incoming.Id == 0)
        {
            await dbContext.Set<TEntity>().AddAsync(incoming, cancellationToken);
            return;
        }

        TEntity? tracked = await dbContext.Set<TEntity>().FindAsync([incoming.Id], cancellationToken);
        if (tracked is null)
        {
            await dbContext.Set<TEntity>().AddAsync(incoming, cancellationToken);
            return;
        }

        applyValues(incoming, tracked);
        tracked.Touch();
    }

    private async Task DeleteByIdAsync<TEntity>(int id, CancellationToken cancellationToken)
        where TEntity : Entity
    {
        if (id == 0)
        {
            return;
        }

        TEntity? tracked = await dbContext.Set<TEntity>().FindAsync([id], cancellationToken);
        if (tracked is not null)
        {
            dbContext.Set<TEntity>().Remove(tracked);
        }
    }
}
