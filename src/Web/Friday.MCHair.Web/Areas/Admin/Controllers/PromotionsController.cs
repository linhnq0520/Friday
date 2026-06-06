using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class PromotionsController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await repository.GetAllPromotionsAsync(cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        Promotion model =
            id.HasValue
                ? await repository.GetPromotionByIdAsync(id.Value, cancellationToken) ?? new Promotion()
                : new Promotion { IsPublished = true, PublishedAt = DateTime.UtcNow };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Promotion model,
        IFormFile? imageFile,
        CancellationToken cancellationToken
    )
    {
        Promotion? existing =
            model.Id > 0 ? await repository.GetPromotionByIdAsync(model.Id, cancellationToken) : null;

        try
        {
            model.ImageUrl = await this.ResolveImageUrlAsync(
                imageFile,
                "khuyen_mai",
                existing?.ImageUrl,
                model.ImageUrl,
                cancellationToken
            );
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        await repository.AddPromotionAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu khuyến mãi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        Promotion? item = await repository.GetPromotionByIdAsync(id, cancellationToken);
        if (item is not null)
        {
            await repository.DeletePromotionAsync(item, cancellationToken);
            await CommitAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }
}
