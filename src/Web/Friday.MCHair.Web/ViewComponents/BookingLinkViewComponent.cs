using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.ViewComponents;

public sealed class BookingLinkViewComponent(ISalonRepository repository) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(
        string label = "Đặt lịch",
        string? cssClass = null,
        int? serviceId = null,
        int? stylistId = null
    )
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            HttpContext.RequestAborted
        );

        BookingLinkViewModel model = BookingSettings.CreateLink(
            settings,
            label,
            cssClass,
            serviceId,
            stylistId
        );

        return View(model);
    }
}
