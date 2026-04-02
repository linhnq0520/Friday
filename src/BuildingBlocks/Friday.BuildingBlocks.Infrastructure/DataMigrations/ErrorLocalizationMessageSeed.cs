using Friday.BuildingBlocks.Application.Errors;

namespace Friday.BuildingBlocks.Infrastructure.DataMigrations;

/// <summary>
/// Reference rows for <c>localization.error_messages</c>. Replace or extend this list; future migrations can append new versions.
/// </summary>
internal static class ErrorLocalizationMessageSeed
{
    internal readonly record struct Row(
        string Module,
        string ErrorCode,
        string Language,
        string Message
    );

    internal static IReadOnlyList<Row> DefaultRows { get; } =
        [
            new("admin", ErrorCodes.Admin.UserNotFound, "en", "User was not found."),
            new("admin", ErrorCodes.Admin.UserNotFound, "vi", "Khong tim thay nguoi dung."),
            new(
                "common",
                ErrorCodes.Common.InternalServerError,
                "en",
                "An unexpected error occurred."
            ),
            new("common", ErrorCodes.Common.InternalServerError, "vi", "Da xay ra loi he thong."),
            new("common", ErrorCodes.Common.NotFound, "en", "Resource was not found."),
            new("common", ErrorCodes.Common.NotFound, "vi", "Khong tim thay du lieu."),
            new("common", ErrorCodes.Common.BadRequest, "en", "Request data is invalid."),
            new("common", ErrorCodes.Common.BadRequest, "vi", "Du lieu gui len khong hop le."),
        ];
}
