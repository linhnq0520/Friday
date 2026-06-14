using Friday.Modules.Salon.Application.Models;
using Friday.MCHair.Web.Localization;

namespace Friday.MCHair.Web.Models;

public static class LocalizationExtensions
{
    public static SiteSectionDto? LocalizeSection(SiteSectionDto? section)
    {
        if (section is null || !CultureHelper.IsEnglish)
        {
            return section;
        }

        SectionEnglishContent? en = SectionContentEn.TryGet(section.SectionKey);
        if (en is null)
        {
            return section;
        }

        return section with
        {
            Title = en.Title ?? section.Title,
            Subtitle = en.Subtitle ?? section.Subtitle,
            Body = en.Body ?? section.Body,
        };
    }
}
