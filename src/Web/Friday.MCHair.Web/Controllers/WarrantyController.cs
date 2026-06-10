using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class WarrantyController(
    ISalonRepository repository,
    IWarrantyStore warrantyStore
) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        WarrantyPageData page = await warrantyStore.GetAsync(cancellationToken);
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        ViewData["Title"] =
            $"{page.Title} | {settings.GetValueOrDefault("site_name", SeoDefaults.SiteName)}";
        ViewData["MetaDescription"] = string.IsNullOrWhiteSpace(page.MetaDescription)
            ? "Chính sách bảo hành dịch vụ làm tóc tại MC Hair Salon – cam kết chất lượng và hài lòng khách hàng."
            : page.MetaDescription;
        return View(page);
    }
}
