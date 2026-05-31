using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class GalleryController(IMediator mediator, ISalonRepository repository) : Controller
{
    public async Task<IActionResult> Index(GalleryCategory? category, CancellationToken cancellationToken)
    {
        IReadOnlyList<GalleryItemDto> items = await mediator.QueryAsync(
            new GetGalleryPageQuery(category),
            cancellationToken
        );
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        ViewData["Title"] = $"Bộ sưu tập | {settings.GetValueOrDefault("site_name", SeoDefaults.SiteName)}";
        ViewBag.Category = category;
        return View(items);
    }
}
