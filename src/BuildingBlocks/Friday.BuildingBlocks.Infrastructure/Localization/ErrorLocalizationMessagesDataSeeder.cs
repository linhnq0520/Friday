using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Seeding;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Friday.BuildingBlocks.Infrastructure.Localization;

public sealed class ErrorLocalizationMessagesDataSeeder(
    FridayDbContext dbContext,
    ILogger<ErrorLocalizationMessagesDataSeeder> logger
) : IDataSeeder
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        foreach (ErrorLocalizationMessage row in GetDefaultRows())
        {
            ErrorLocalizationMessage? existing = await dbContext
                .Set<ErrorLocalizationMessage>()
                .FirstOrDefaultAsync(
                    x =>
                        x.Module == row.Module
                        && x.ErrorCode == row.ErrorCode
                        && x.Language == row.Language,
                    cancellationToken
                );

            if (existing is null)
            {
                row.UpdatedOnUtc = now;
                dbContext.Add(row);
            }
            else if (!string.Equals(existing.Message, row.Message, StringComparison.Ordinal))
            {
                existing.Message = row.Message;
                existing.UpdatedOnUtc = now;
            }
        }

        int written = await dbContext.SaveChangesAsync(cancellationToken);
        if (written > 0)
        {
            logger.LogInformation(
                "Seeded or updated {Count} localization.error_messages row(s).",
                written
            );
        }
    }

    private static IEnumerable<ErrorLocalizationMessage> GetDefaultRows()
    {
        yield return new ErrorLocalizationMessage
        {
            Module = "admin",
            ErrorCode = ErrorCodes.Admin.UserNotFound,
            Language = "en",
            Message = "User was not found.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "admin",
            ErrorCode = ErrorCodes.Admin.UserNotFound,
            Language = "vi",
            Message = "Khong tim thay nguoi dung.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "common",
            ErrorCode = ErrorCodes.Common.InternalServerError,
            Language = "en",
            Message = "An unexpected error occurred.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "common",
            ErrorCode = ErrorCodes.Common.InternalServerError,
            Language = "vi",
            Message = "Da xay ra loi he thong.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "common",
            ErrorCode = ErrorCodes.Common.NotFound,
            Language = "en",
            Message = "Resource was not found.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "common",
            ErrorCode = ErrorCodes.Common.NotFound,
            Language = "vi",
            Message = "Khong tim thay du lieu.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "common",
            ErrorCode = ErrorCodes.Common.BadRequest,
            Language = "en",
            Message = "Request data is invalid.",
        };
        yield return new ErrorLocalizationMessage
        {
            Module = "common",
            ErrorCode = ErrorCodes.Common.BadRequest,
            Language = "vi",
            Message = "Du lieu gui len khong hop le.",
        };
    }
}
