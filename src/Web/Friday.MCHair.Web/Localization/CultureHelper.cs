using System.Globalization;

namespace Friday.MCHair.Web.Localization;

public static class CultureHelper
{
    public static bool IsEnglish =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals(
            CultureConstants.English,
            StringComparison.OrdinalIgnoreCase
        );
}
