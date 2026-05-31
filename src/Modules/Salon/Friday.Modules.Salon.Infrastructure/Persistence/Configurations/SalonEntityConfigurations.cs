using Friday.Modules.Salon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friday.Modules.Salon.Infrastructure.Persistence.Configurations;

internal static class SalonTable
{
    internal const string Prefix = "salon_";
}

public sealed class HairServiceConfiguration : IEntityTypeConfiguration<HairService>
{
    public void Configure(EntityTypeBuilder<HairService> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}services");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.PriceFrom).HasPrecision(18, 2);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class StylistConfiguration : IEntityTypeConfiguration<Stylist>
{
    public void Configure(EntityTypeBuilder<Stylist> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}stylists");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200);
        builder.Property(x => x.Bio).HasMaxLength(2000);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class GalleryItemConfiguration : IEntityTypeConfiguration<GalleryItem>
{
    public void Configure(EntityTypeBuilder<GalleryItem> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}gallery");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}promotions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Summary).HasMaxLength(1000);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class TestimonialConfiguration : IEntityTypeConfiguration<Testimonial>
{
    public void Configure(EntityTypeBuilder<Testimonial> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}testimonials");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Content).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class SiteSectionConfiguration : IEntityTypeConfiguration<SiteSection>
{
    public void Configure(EntityTypeBuilder<SiteSection> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}sections");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SectionKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(300);
        builder.Property(x => x.Subtitle).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);
        builder.HasIndex(x => x.SectionKey).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class BeforeAfterItemConfiguration : IEntityTypeConfiguration<BeforeAfterItem>
{
    public void Configure(EntityTypeBuilder<BeforeAfterItem> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}before_after");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.BeforeImageUrl).HasMaxLength(500).IsRequired();
        builder.Property(x => x.AfterImageUrl).HasMaxLength(500).IsRequired();
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}appointments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(30).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.HairService).WithMany().HasForeignKey(x => x.HairServiceId);
        builder.HasOne(x => x.Stylist).WithMany().HasForeignKey(x => x.StylistId);
        builder.HasIndex(x => x.ScheduledAt);
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class SiteSettingConfiguration : IEntityTypeConfiguration<SiteSetting>
{
    public void Configure(EntityTypeBuilder<SiteSetting> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}settings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SettingKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(2000).IsRequired();
        builder.HasIndex(x => x.SettingKey).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}

public sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.ToTable($"{SalonTable.Prefix}admin_users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Username).HasMaxLength(100).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Username).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}
