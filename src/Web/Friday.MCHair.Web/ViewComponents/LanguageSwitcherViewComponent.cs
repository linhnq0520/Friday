using Friday.MCHair.Web.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.ViewComponents;

public sealed class LanguageSwitcherViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        string current = CultureHelper.IsEnglish
            ? CultureConstants.English
            : CultureConstants.Vietnamese;

        return View("Default", current);
    }
}
