using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class AboutController(ISalonRepository repository) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        ViewData["Title"] =
            $"Giới thiệu | {settings.GetValueOrDefault("site_name", SeoDefaults.SiteName)}";
        ViewData["MetaDescription"] =
            "Sứ mệnh, tầm nhìn và giá trị cốt lõi của MC Hair Salon – salon làm tóc hiện đại tại TP.HCM.";
        return View();
    }
}
