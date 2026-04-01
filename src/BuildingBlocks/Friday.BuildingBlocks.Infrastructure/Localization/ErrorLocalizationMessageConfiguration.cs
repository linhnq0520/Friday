using Friday.BuildingBlocks.Application.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friday.BuildingBlocks.Infrastructure.Localization;

public sealed class ErrorLocalizationMessageConfiguration : IEntityTypeConfiguration<ErrorLocalizationMessage>
{
    public void Configure(EntityTypeBuilder<ErrorLocalizationMessage> builder)
    {
        builder.ToTable("error_messages", "localization");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Module).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ErrorCode).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Language).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(500).IsRequired();
        builder.Property(x => x.UpdatedOnUtc).IsRequired();

        builder.HasIndex(x => new
        {
            x.Module,
            x.ErrorCode,
            x.Language
        }).IsUnique();

        builder.HasData(
            new ErrorLocalizationMessage
            {
                Id = 1,
                Module = "admin",
                ErrorCode = ErrorCodes.Admin.UserNotFound,
                Language = "en",
                Message = "User was not found."
            },
            new ErrorLocalizationMessage
            {
                Id = 2,
                Module = "admin",
                ErrorCode = ErrorCodes.Admin.UserNotFound,
                Language = "vi",
                Message = "Khong tim thay nguoi dung."
            },
            new ErrorLocalizationMessage
            {
                Id = 3,
                Module = "common",
                ErrorCode = ErrorCodes.Common.InternalServerError,
                Language = "en",
                Message = "An unexpected error occurred."
            },
            new ErrorLocalizationMessage
            {
                Id = 4,
                Module = "common",
                ErrorCode = ErrorCodes.Common.InternalServerError,
                Language = "vi",
                Message = "Da xay ra loi he thong."
            }
        );
    }
}
