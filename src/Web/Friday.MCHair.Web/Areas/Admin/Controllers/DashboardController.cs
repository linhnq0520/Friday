using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Domain.Enums;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class DashboardController(IMediator mediator) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        DateTime today = DateTime.Today;
        IReadOnlyList<Friday.Modules.Salon.Application.Models.AppointmentDto> todayAppointments =
            await mediator.QueryAsync(
                new GetAdminAppointmentsQuery(today, today.AddDays(1).AddTicks(-1), null),
                cancellationToken
            );
        ViewBag.TodayCount = todayAppointments.Count;
        ViewBag.PendingCount = todayAppointments.Count(x => x.Status == AppointmentStatus.Pending);
        return View(todayAppointments);
    }
}
