using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
public sealed class SettingsController(ISalonRepository repository) : AdminControllerBase
{
    private static readonly string[] Keys =
    [
        "site_name",
        "tagline",
        "hotline",
        "email",
        "address",
        "address_short",
        "maps_url",
        "opening_hours",
        "facebook",
        "zalo",
        "messenger_url",
        "instagram",
        "seo_title",
        "seo_description",
        "booking_mode",
        "booking_external_url",
    ];

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(IFormCollection form, CancellationToken cancellationToken)
    {
        foreach (string key in Keys)
        {
            string value = form[key].ToString();
            await repository.UpsertSettingAsync(key, value, cancellationToken);
        }

        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu cài đặt website.";
        return RedirectToAction(nameof(Index));
    }
}
