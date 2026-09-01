namespace Friday.MCHair.Web.Models;

public sealed class SiteInfoViewModel
{
    public string Hotline { get; init; } = SiteContent.DefaultHotline;

    public string Address { get; init; } = SiteContent.DefaultAddress;

    public string AddressShort { get; init; } = SiteContent.DefaultAddressShort;

    public string OpeningHours { get; init; } = SiteContent.DefaultOpeningHours;

    public string FacebookUrl { get; init; } = SiteContent.FacebookUrl;
    public string YouTubeUrl { get; init; } = SiteContent.YouTubeUrl;

    public string MapsUrl { get; init; } = SiteContent.DefaultMapsUrl;

    public string ZaloUrl { get; init; } = string.Empty;

    public string MessengerUrl { get; init; } = SiteContent.DefaultMessengerUrl;

    public string PhoneTel { get; init; } = string.Empty;
}
