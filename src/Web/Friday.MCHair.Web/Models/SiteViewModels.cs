using Friday.MCHair.Web.Localization;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Enums;

namespace Friday.MCHair.Web.Models;

public sealed class HomeIndexViewModel
{
    public required HomePageDto Page { get; init; }

    public string GetSetting(string key, string fallback = "") =>
        Page.Settings.TryGetValue(key, out string? value) ? value : fallback;

    public SiteSectionDto? GetSection(string key) =>
        Page.Sections.FirstOrDefault(x =>
            x.SectionKey.Equals(key, StringComparison.OrdinalIgnoreCase)
        );
}

public sealed class ServicesIndexViewModel
{
    public required IReadOnlyList<HairServiceDto> Services { get; init; }
    public required IReadOnlyDictionary<string, string> Settings { get; init; }
    public required PriceListData PriceList { get; init; }
}

public sealed class ServicesHubViewModel
{
    public required IReadOnlyList<HairServiceDto> Services { get; init; }
    public required IReadOnlyDictionary<string, string> Settings { get; init; }
    public required IReadOnlyList<GalleryItemDto> GalleryItems { get; init; }
}

public sealed class ServiceDetailViewModel
{
    public required HairServiceDto Service { get; init; }
    public required IReadOnlyList<HairServiceDto> OtherServices { get; init; }
    public required IReadOnlyDictionary<string, string> Settings { get; init; }

    public ServicePricingData? PricingData
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Service.PricingTableJson)) return null;
            try
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<ServicePricingData>(Service.PricingTableJson, JsonOptions);
                return NormalizePricingData(data);
            }
            catch
            {
                return null;
            }
        }
    }

    private static ServicePricingData? NormalizePricingData(ServicePricingData? data)
    {
        if (data == null) return null;

        var normalizedGroups = data.Groups.Select(g => new ServicePriceGroup(
            g.Title,
            g.Columns,
            g.Items.Select(i => new ServicePriceItem(
                i.Name,
                NormalizePriceString(i.Price1),
                NormalizePriceString(i.Price2),
                i.Note
            )).ToList()
        )).ToList();

        var normalizedAddons = data.Addons.Select(a => new ServicePriceAddon(
            a.Name,
            NormalizePriceString(a.Price) ?? a.Price
        )).ToList();

        return new ServicePricingData(normalizedGroups, normalizedAddons, data.Note);
    }

    public static string? NormalizePriceString(string? price)
    {
        if (string.IsNullOrWhiteSpace(price)) return price;
        string s = price.Trim();

        // If it already has .000 or 5+ digits or is formatted
        if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\b\d{1,3}\.000\b") || 
            System.Text.RegularExpressions.Regex.IsMatch(s, @"\b\d{5,}\b") ||
            s.Contains("000đ", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("000 đ", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("000 VNĐ", StringComparison.OrdinalIgnoreCase))
        {
            return s;
        }

        string unitSuffix = "";
        int slashIdx = s.IndexOf('/');
        if (slashIdx >= 0)
        {
            unitSuffix = " " + s.Substring(slashIdx).Trim();
            s = s.Substring(0, slashIdx).Trim();
        }

        // Strip trailing 'đ'
        s = System.Text.RegularExpressions.Regex.Replace(s, @"\s*đ\s*$", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // 1. Handle X.YYY (e.g. 1.000, 1.200) -> X.YYY.000
        string res = System.Text.RegularExpressions.Regex.Replace(s, @"\b(\d{1,2})\.(\d{3})\b", "$1.$2.000");

        // 2. Handle standalone numbers 1 to 3 digits (e.g. 50, 100, 250, 350)
        res = System.Text.RegularExpressions.Regex.Replace(res, @"(?<!\.)\b(\d{1,3})\b(?!\.\d)", "$1.000");

        if (!res.Contains('đ') && !res.Contains("VNĐ", StringComparison.OrdinalIgnoreCase) && !res.Contains('k', StringComparison.OrdinalIgnoreCase))
        {
            res += "đ";
        }

        if (!string.IsNullOrEmpty(unitSuffix))
        {
            res += unitSuffix;
        }

        return res;
    }

    public IReadOnlyList<ServiceMethodItem> Methods
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Service.MethodsJson)) return [];
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<IReadOnlyList<ServiceMethodItem>>(Service.MethodsJson, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    public IReadOnlyList<ServiceStepItem> Steps
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Service.StepsJson)) return [];
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<IReadOnlyList<ServiceStepItem>>(Service.StepsJson, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    public IReadOnlyList<ServiceFaqItem> Faqs
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Service.FaqsJson)) return [];
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<IReadOnlyList<ServiceFaqItem>>(Service.FaqsJson, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}

public sealed class BookingViewModel
{
    public required BookingFormDto Form { get; init; }
    public int? PreselectedServiceId { get; init; }
    public int? PreselectedStylistId { get; init; }
    public string? SuccessMessage { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class BookingInputModel
{
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int HairServiceId { get; set; }
    public int? StylistId { get; set; }
    public DateTime ScheduledDate { get; set; } = DateTime.Today.AddDays(1);
    public string ScheduledTime { get; set; } = "10:00";
    public string? Notes { get; set; }
}

public static class ShowcaseTypeLabels
{
    public static string GetLabel(ShowcaseType type) =>
        type switch
        {
            ShowcaseType.Feedback => "Feedback",
            ShowcaseType.BeforeAfter => "Before & After",
            _ => type.ToString(),
        };
}

public static class GalleryCategoryLabels
{
    public static string GetLabel(GalleryCategory category) =>
        CultureHelper.IsEnglish ? GetLabelEn(category) : GetLabelVi(category);

    private static string GetLabelVi(GalleryCategory category) =>
        category switch
        {
            GalleryCategory.FashionColor => "Màu thời trang",
            GalleryCategory.TrendingStyle => "Kiểu tóc thịnh hành",
            GalleryCategory.HairRecovery => "Phục hồi hư tổn",
            GalleryCategory.HairExtensions => "Nối tóc",
            GalleryCategory.BeforeAfter => "Before & After",
            _ => category.ToString(),
        };

    private static string GetLabelEn(GalleryCategory category) =>
        category switch
        {
            GalleryCategory.FashionColor => "Fashion color",
            GalleryCategory.TrendingStyle => "Trending styles",
            GalleryCategory.HairRecovery => "Damage repair",
            GalleryCategory.HairExtensions => "Hair extensions",
            GalleryCategory.BeforeAfter => "Before & After",
            _ => category.ToString(),
        };
}

public static class SeoDefaults
{
    public const string SiteName = "MC Hair Salon";
    public const string GoogleSiteVerification = "Wl2m3g0qP_w8DCxyMUrNzxEzA4QLjdwvypX-nh5P0hQ";
    public const string GoogleTagManagerId = "GTM-WRQRXXBR";
}
