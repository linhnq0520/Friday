using Friday.Modules.Salon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Friday.Modules.Salon.Infrastructure.Persistence;

public sealed class SalonDbContext(DbContextOptions<SalonDbContext> options) : DbContext(options)
{
    public DbSet<HairService> HairServices => Set<HairService>();
    public DbSet<Stylist> Stylists => Set<Stylist>();
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Testimonial> Testimonials => Set<Testimonial>();
    public DbSet<SiteSection> SiteSections => Set<SiteSection>();
    public DbSet<BeforeAfterItem> BeforeAfterItems => Set<BeforeAfterItem>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalonDbContext).Assembly);
    }
}
