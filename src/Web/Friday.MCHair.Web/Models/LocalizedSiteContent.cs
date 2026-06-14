using Friday.MCHair.Web.Localization;

namespace Friday.MCHair.Web.Models;

public static class LocalizedSiteContent
{
    public static string AboutStoryTitle =>
        CultureHelper.IsEnglish ? SiteContentEn.AboutStoryTitle : SiteContent.AboutStoryTitle;

    public static string AboutTagline =>
        CultureHelper.IsEnglish ? SiteContentEn.AboutTagline : SiteContent.AboutTagline;

    public static string AboutStoryBody =>
        CultureHelper.IsEnglish ? SiteContentEn.AboutStoryBody : SiteContent.AboutStoryBody;

    public static string MissionTitle =>
        CultureHelper.IsEnglish ? SiteContentEn.MissionTitle : SiteContent.MissionTitle;

    public static string MissionBody =>
        CultureHelper.IsEnglish ? SiteContentEn.MissionBody : SiteContent.MissionBody;

    public static string VisionTitle =>
        CultureHelper.IsEnglish ? SiteContentEn.VisionTitle : SiteContent.VisionTitle;

    public static string VisionBody =>
        CultureHelper.IsEnglish ? SiteContentEn.VisionBody : SiteContent.VisionBody;

    public static CoreValueItem[] CoreValues =>
        CultureHelper.IsEnglish ? SiteContentEn.CoreValues : SiteContent.CoreValues;
}
