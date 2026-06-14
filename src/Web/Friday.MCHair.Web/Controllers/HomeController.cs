using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class HomeController(
    IMediator mediator,
    IUiLocalizer localizer
) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        HomePageDto page = await mediator.QueryAsync(new GetHomePageQuery(), cancellationToken);
        ViewData["Title"] = page.Settings.GetValueOrDefault(
            "seo_title",
            localizer["Meta_Home"].Value
        );
        ViewData["MetaDescription"] = page.Settings.GetValueOrDefault(
            "seo_description",
            string.Empty
        );
        ViewData["ShowBackToTop"] = true;
        return View(new HomeIndexViewModel { Page = page });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
