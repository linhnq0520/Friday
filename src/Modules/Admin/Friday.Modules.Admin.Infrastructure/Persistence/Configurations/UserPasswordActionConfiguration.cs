using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friday.Modules.Admin.Infrastructure.Persistence.Configurations;

public sealed class UserPasswordActionConfiguration : IEntityTypeConfiguration<UserPasswordAction>
{
    public void Configure(EntityTypeBuilder<UserPasswordAction> builder)
    {
        builder.ToTable("user_password_actions", "admin");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActionType).HasMaxLength(32).IsRequired();
        builder.Property(x => x.TokenHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).IsRequired();
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ConsumedAtUtc);

        builder.HasIndex(x => x.TokenHash).IsUnique();
        builder.HasIndex(x => new { x.UserId, x.ActionType });
    }
}
