using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class NewsController(
    IMediator mediator,
    IUiLocalizer localizer
) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<PromotionDto> items = await mediator.QueryAsync(
            new GetPromotionsPageQuery(),
            cancellationToken
        );
        ViewData["Title"] = localizer["Meta_News"].Value;
        return View(items);
    }
}
