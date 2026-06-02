namespace Friday.MCHair.Web.Models;

public sealed class SiteInfoViewModel
{
    public string Hotline { get; init; } = SiteContent.DefaultHotline;

    public string Address { get; init; } = SiteContent.DefaultAddress;

    public string AddressShort { get; init; } = SiteContent.DefaultAddressShort;

    public string OpeningHours { get; init; } = SiteContent.DefaultOpeningHours;

    public string FacebookUrl { get; init; } = SiteContent.FacebookUrl;

    public string MapsUrl { get; init; } = SiteContent.DefaultMapsUrl;
}
