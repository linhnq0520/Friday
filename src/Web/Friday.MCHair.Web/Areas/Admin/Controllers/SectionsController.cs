using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class SectionsController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await repository.GetAllSectionsAsync(cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        SiteSection model =
            id.HasValue
                ? await repository.GetSectionByIdAsync(id.Value, cancellationToken) ?? new SiteSection()
                : new SiteSection { IsVisible = true };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SiteSection model, CancellationToken cancellationToken)
    {
        await repository.AddSectionAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu nội dung section.";
        return RedirectToAction(nameof(Index));
    }
}
