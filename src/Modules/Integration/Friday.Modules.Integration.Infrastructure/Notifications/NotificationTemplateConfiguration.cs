using Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friday.Modules.Integration.Infrastructure.Notifications;

public sealed class NotificationTemplateConfiguration
    : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates", "integration");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Channel).HasMaxLength(32).IsRequired();
        builder.Property(x => x.TemplateCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(10).IsRequired();
        builder.Property(x => x.SubjectTemplate).HasMaxLength(500).IsRequired();
        builder.Property(x => x.BodyTemplate).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.UpdatedOnUtc).IsRequired();

        builder
            .HasIndex(x => new
            {
                x.Channel,
                x.TemplateCode,
                x.Language,
                x.Version,
            })
            .IsUnique();
        builder.HasIndex(x => new
        {
            x.Channel,
            x.TemplateCode,
            x.Language,
            x.IsActive,
        });
    }
}
