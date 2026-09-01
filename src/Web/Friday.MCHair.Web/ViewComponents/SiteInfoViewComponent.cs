using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.ViewComponents;

public sealed class SiteInfoViewComponent(ISalonRepository repository) : ViewComponent
{
    private const string CacheKey = "SiteInfoViewModel";

    public async Task<IViewComponentResult> InvokeAsync(string? template = null)
    {
        SiteInfoViewModel model = await GetSiteInfoAsync(HttpContext.RequestAborted);
        return string.IsNullOrWhiteSpace(template) ? View(model) : View(template, model);
    }

    private async Task<SiteInfoViewModel> GetSiteInfoAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Items[CacheKey] is SiteInfoViewModel cached)
        {
            return cached;
        }

        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );

        string address = Get(settings, "address", SiteContent.DefaultAddress);
        string hotline = Get(settings, "hotline", SiteContent.DefaultHotline);
        SiteInfoViewModel model = new()
        {
            Hotline = hotline,
            Address = address,
            AddressShort = Get(settings, "address_short", SiteContent.DefaultAddressShort),
            OpeningHours = Get(settings, "opening_hours", SiteContent.DefaultOpeningHours),
            FacebookUrl = Get(settings, "facebook", SiteContent.FacebookUrl),
            YouTubeUrl = Get(settings, "youtube", SiteContent.YouTubeUrl),
            MapsUrl = Get(settings, "maps_url", SiteContent.DefaultMapsUrl),
            ZaloUrl = BuildZaloUrl(Get(settings, "zalo", SiteContent.DefaultZaloPhone)),
            MessengerUrl = Get(settings, "messenger_url", SiteContent.DefaultMessengerUrl),
            PhoneTel = NormalizePhone(hotline),
        };

        HttpContext.Items[CacheKey] = model;
        return model;
    }

    private static string Get(
        IReadOnlyDictionary<string, string> settings,
        string key,
        string fallback
    ) => settings.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value)
        ? value
        : fallback;

    private static string BuildZaloUrl(string zalo)
    {
        if (zalo.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return zalo;
        }

        return $"https://zalo.me/{NormalizePhone(zalo)}";
    }

    private static string NormalizePhone(string phone) =>
        new string(phone.Where(char.IsDigit).ToArray());
}
