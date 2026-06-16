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
