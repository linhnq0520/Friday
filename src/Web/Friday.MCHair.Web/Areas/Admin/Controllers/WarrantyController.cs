using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class WarrantyController(IWarrantyStore warrantyStore) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        return View(await warrantyStore.GetAsync(cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        WarrantyPageData model,
        CancellationToken cancellationToken
    )
    {
        await warrantyStore.SaveAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu nội dung bảo hành.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reset(CancellationToken cancellationToken)
    {
        await warrantyStore.SaveAsync(WarrantyDefaults.Create(), cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã khôi phục nội dung bảo hành mặc định.";
        return RedirectToAction(nameof(Index));
    }
}
