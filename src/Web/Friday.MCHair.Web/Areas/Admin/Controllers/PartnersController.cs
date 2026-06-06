using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class PartnersController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await repository.GetAllPartnersAsync(cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        Partner model =
            id.HasValue
                ? await repository.GetPartnerByIdAsync(id.Value, cancellationToken) ?? new Partner()
                : new Partner { IsActive = true };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Partner model,
        IFormFile? imageFile,
        CancellationToken cancellationToken
    )
    {
        Partner? existing =
            model.Id > 0
                ? await repository.GetPartnerByIdAsync(model.Id, cancellationToken)
                : null;

        try
        {
            model.LogoUrl = await this.ResolveImageUrlAsync(
                imageFile,
                "partners",
                existing?.LogoUrl,
                model.LogoUrl,
                cancellationToken
            );
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        await repository.AddPartnerAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu đối tác.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        Partner? item = await repository.GetPartnerByIdAsync(id, cancellationToken);
        if (item is not null)
        {
            await repository.DeletePartnerAsync(item, cancellationToken);
            await CommitAsync(cancellationToken);
        }

        return RedirectToAction(nameof(Index));
    }
}
