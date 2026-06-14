using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class AboutController(IUiLocalizer localizer) : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = localizer["Meta_About"].Value;
        ViewData["MetaDescription"] = localizer["Meta_AboutDescription"].Value;
        return View();
    }
}
