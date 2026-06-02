using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class PriceListController(IPriceListStore priceListStore) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        PriceListData data = await priceListStore.GetAsync(cancellationToken);
        data.Groups = data
            .Groups.OrderBy(g => g.ColumnIndex)
            .ThenBy(g => g.SortOrder)
            .ToList();
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        PriceListData model,
        CancellationToken cancellationToken
    )
    {
        await priceListStore.SaveAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu bảng giá.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reset(CancellationToken cancellationToken)
    {
        await priceListStore.SaveAsync(PriceListDefaults.Create(), cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã khôi phục bảng giá mặc định.";
        return RedirectToAction(nameof(Index));
    }
}
