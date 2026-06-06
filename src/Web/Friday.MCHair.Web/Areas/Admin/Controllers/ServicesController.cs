using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class ServicesController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<HairService> items = await repository.GetAllServicesAsync(cancellationToken);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        HairService model =
            id.HasValue
                ? await repository.GetServiceByIdAsync(id.Value, cancellationToken)
                    ?? new HairService()
                : new HairService { IsActive = true, RatingDisplay = 5 };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        HairService model,
        IFormFile? imageFile,
        CancellationToken cancellationToken
    )
    {
        HairService? existing =
            model.Id > 0 ? await repository.GetServiceByIdAsync(model.Id, cancellationToken) : null;

        try
        {
            model.ImageUrl = await this.ResolveImageUrlAsync(
                imageFile,
                "dich_vu",
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

        await repository.AddServiceAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu dịch vụ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        HairService? item = await repository.GetServiceByIdAsync(id, cancellationToken);
        if (item is not null)
        {
            await repository.DeleteServiceAsync(item, cancellationToken);
            await CommitAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }
}
