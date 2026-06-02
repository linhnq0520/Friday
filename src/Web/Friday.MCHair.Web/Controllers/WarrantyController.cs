using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class WarrantyController(ISalonRepository repository) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        ViewData["Title"] =
            $"Chế độ bảo hành | {settings.GetValueOrDefault("site_name", SeoDefaults.SiteName)}";
        ViewData["MetaDescription"] =
            "Chính sách bảo hành dịch vụ làm tóc tại MC Hair Salon – cam kết chất lượng và hài lòng khách hàng.";
        return View();
    }
}
