using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class StylistsController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await repository.GetAllStylistsAsync(cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        Stylist model =
            id.HasValue
                ? await repository.GetStylistByIdAsync(id.Value, cancellationToken) ?? new Stylist()
                : new Stylist { IsActive = true };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Stylist model, CancellationToken cancellationToken)
    {
        await repository.AddStylistAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu thợ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        Stylist? item = await repository.GetStylistByIdAsync(id, cancellationToken);
        if (item is not null)
        {
            await repository.DeleteStylistAsync(item, cancellationToken);
            await CommitAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }
}
