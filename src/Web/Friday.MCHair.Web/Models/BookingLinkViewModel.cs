namespace Friday.MCHair.Web.Models;

public sealed class BookingLinkViewModel
{
    public string Label { get; init; } = "Đặt lịch";

    public string? CssClass { get; init; }

    public bool UseExternal { get; init; }

    public string? ExternalUrl { get; init; }

    public int? ServiceId { get; init; }

    public int? StylistId { get; init; }
}
