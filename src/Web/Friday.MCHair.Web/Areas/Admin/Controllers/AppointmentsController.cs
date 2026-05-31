using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Enums;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class AppointmentsController(IMediator mediator) : AdminControllerBase
{
    public async Task<IActionResult> Index(
        DateTime? from,
        DateTime? to,
        AppointmentStatus? status,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<AppointmentDto> items = await mediator.QueryAsync(
            new GetAdminAppointmentsQuery(from, to, status),
            cancellationToken
        );
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");
        ViewBag.Status = status;
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(
        int id,
        AppointmentStatus status,
        CancellationToken cancellationToken
    )
    {
        await mediator.SendAsync(new UpdateAppointmentStatusCommand(id, status), cancellationToken);
        TempData["Success"] = "Đã cập nhật trạng thái lịch hẹn.";
        return RedirectToAction(nameof(Index));
    }
}
