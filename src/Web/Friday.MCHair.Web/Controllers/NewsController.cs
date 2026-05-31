using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class NewsController(IMediator mediator, ISalonRepository repository) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<PromotionDto> items = await mediator.QueryAsync(
            new GetPromotionsPageQuery(),
            cancellationToken
        );
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        ViewData["Title"] = $"Tin tức & khuyến mãi | {settings.GetValueOrDefault("site_name", SeoDefaults.SiteName)}";
        return View(items);
    }
}
