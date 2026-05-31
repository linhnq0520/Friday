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

    public async Task AddServiceAsync(HairService service, CancellationToken cancellationToken = default)
    {
        if (service.Id == 0)
        {
            await dbContext.Set<HairService>().AddAsync(service, cancellationToken);
        }
        else
        {
            dbContext.Set<HairService>().Update(service);
        }
    }

    public Task DeleteServiceAsync(HairService service, CancellationToken cancellationToken = default)
    {
        dbContext.Set<HairService>().Remove(service);
        return Task.CompletedTask;
    }

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

    public async Task AddStylistAsync(Stylist stylist, CancellationToken cancellationToken = default)
    {
        if (stylist.Id == 0)
        {
            await dbContext.Set<Stylist>().AddAsync(stylist, cancellationToken);
        }
        else
        {
            dbContext.Set<Stylist>().Update(stylist);
        }
    }

    public Task DeleteStylistAsync(Stylist stylist, CancellationToken cancellationToken = default)
    {
        dbContext.Set<Stylist>().Remove(stylist);
        return Task.CompletedTask;
    }

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

    public async Task AddGalleryItemAsync(GalleryItem item, CancellationToken cancellationToken = default)
    {
        if (item.Id == 0)
        {
            await dbContext.Set<GalleryItem>().AddAsync(item, cancellationToken);
        }
        else
        {
            dbContext.Set<GalleryItem>().Update(item);
        }
    }

    public Task DeleteGalleryItemAsync(GalleryItem item, CancellationToken cancellationToken = default)
    {
        dbContext.Set<GalleryItem>().Remove(item);
        return Task.CompletedTask;
    }

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

    public async Task AddPromotionAsync(Promotion promotion, CancellationToken cancellationToken = default)
    {
        if (promotion.Id == 0)
        {
            await dbContext.Set<Promotion>().AddAsync(promotion, cancellationToken);
        }
        else
        {
            dbContext.Set<Promotion>().Update(promotion);
        }
    }

    public Task DeletePromotionAsync(Promotion promotion, CancellationToken cancellationToken = default)
    {
        dbContext.Set<Promotion>().Remove(promotion);
        return Task.CompletedTask;
    }

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

    public async Task AddTestimonialAsync(Testimonial testimonial, CancellationToken cancellationToken = default)
    {
        if (testimonial.Id == 0)
        {
            await dbContext.Set<Testimonial>().AddAsync(testimonial, cancellationToken);
        }
        else
        {
            dbContext.Set<Testimonial>().Update(testimonial);
        }
    }

    public Task DeleteTestimonialAsync(Testimonial testimonial, CancellationToken cancellationToken = default)
    {
        dbContext.Set<Testimonial>().Remove(testimonial);
        return Task.CompletedTask;
    }

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

    public async Task AddSectionAsync(SiteSection section, CancellationToken cancellationToken = default)
    {
        if (section.Id == 0)
        {
            await dbContext.Set<SiteSection>().AddAsync(section, cancellationToken);
        }
        else
        {
            dbContext.Set<SiteSection>().Update(section);
        }
    }

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

    public async Task AddBeforeAfterAsync(BeforeAfterItem item, CancellationToken cancellationToken = default)
    {
        if (item.Id == 0)
        {
            await dbContext.Set<BeforeAfterItem>().AddAsync(item, cancellationToken);
        }
        else
        {
            dbContext.Set<BeforeAfterItem>().Update(item);
        }
    }

    public Task DeleteBeforeAfterAsync(BeforeAfterItem item, CancellationToken cancellationToken = default)
    {
        dbContext.Set<BeforeAfterItem>().Remove(item);
        return Task.CompletedTask;
    }

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
            dbContext.Set<SiteSetting>().Update(existing);
        }
    }

    public async Task AddAppointmentAsync(Appointment appointment, CancellationToken cancellationToken = default)
    {
        if (appointment.Id == 0)
        {
            await dbContext.Set<Appointment>().AddAsync(appointment, cancellationToken);
        }
        else
        {
            dbContext.Set<Appointment>().Update(appointment);
        }
    }

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

    public async Task AddAdminUserAsync(AdminUser user, CancellationToken cancellationToken = default)
    {
        if (user.Id == 0)
        {
            await dbContext.Set<AdminUser>().AddAsync(user, cancellationToken);
        }
        else
        {
            dbContext.Set<AdminUser>().Update(user);
        }
    }
}
