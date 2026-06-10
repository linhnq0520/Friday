namespace Friday.MCHair.Web.Models;

public static class BookingSettings
{
    public const string ModeKey = "booking_mode";

    public const string ExternalUrlKey = "booking_external_url";

    public const string ModeInternal = "internal";

    public const string ModeEasySalon = "easysalon";

    public static BookingLinkViewModel CreateLink(
        IReadOnlyDictionary<string, string> settings,
        string label = "Đặt lịch",
        string? cssClass = null,
        int? serviceId = null,
        int? stylistId = null
    )
    {
        bool useExternal = IsExternalMode(settings);
        string externalUrl = GetExternalUrl(settings);

        return new BookingLinkViewModel
        {
            Label = label,
            CssClass = cssClass,
            UseExternal = useExternal,
            ExternalUrl = externalUrl,
            ServiceId = serviceId,
            StylistId = stylistId,
        };
    }

    public static bool IsExternalMode(IReadOnlyDictionary<string, string> settings)
    {
        string mode = GetMode(settings);
        return mode != ModeInternal;
    }

    public static string GetExternalUrl(IReadOnlyDictionary<string, string> settings)
    {
        if (
            settings.TryGetValue(ExternalUrlKey, out string? url)
            && !string.IsNullOrWhiteSpace(url)
        )
        {
            return url.Trim();
        }

        return SiteContent.DefaultBookingEasySalonUrl;
    }

    private static string GetMode(IReadOnlyDictionary<string, string> settings)
    {
        if (
            settings.TryGetValue(ModeKey, out string? mode)
            && !string.IsNullOrWhiteSpace(mode)
        )
        {
            return mode.Trim().ToLowerInvariant();
        }

        return SiteContent.DefaultBookingMode;
    }
}
