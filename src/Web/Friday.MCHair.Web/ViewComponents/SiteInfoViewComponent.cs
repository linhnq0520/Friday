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
        SiteInfoViewModel model = new()
        {
            Hotline = Get(settings, "hotline", SiteContent.DefaultHotline),
            Address = address,
            AddressShort = Get(settings, "address_short", SiteContent.DefaultAddressShort),
            OpeningHours = Get(settings, "opening_hours", SiteContent.DefaultOpeningHours),
            FacebookUrl = Get(settings, "facebook", SiteContent.FacebookUrl),
            MapsUrl = Get(settings, "maps_url", SiteContent.DefaultMapsUrl),
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
}
