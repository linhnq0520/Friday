using Friday.MCHair.Web.Localization;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class PromotionsController(IMediator mediator, IUiLocalizer localizer) : Controller
{
    [HttpGet]
    [Route("khuyen-mai")]
    [Route("uu-dai")]
    [Route("promotions")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<PromotionDto> promotions = await mediator.QueryAsync(
            new GetPromotionsPageQuery(),
            cancellationToken
        );

        ViewData["Title"] = localizer["Meta_Promotions"].Value;
        ViewData["MetaDescription"] = localizer["Meta_PromotionsDescription"].Value;

        return View(promotions);
    }
}
