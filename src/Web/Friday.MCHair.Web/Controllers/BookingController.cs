using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Localization;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class BookingController(
    IMediator mediator,
    ISalonRepository repository,
    IUiLocalizer localizer
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        int? serviceId,
        int? stylistId,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );

        if (BookingSettings.IsExternalMode(settings))
        {
            return Redirect(BookingSettings.GetExternalUrl(settings));
        }

        BookingFormDto form = await mediator.QueryAsync(new GetBookingFormQuery(), cancellationToken);
        ViewData["Title"] = localizer["Meta_Booking"].Value;
        ViewData["MetaDescription"] = localizer["Meta_BookingDescription"].Value;
        return View(
            new BookingViewModel
            {
                Form = form,
                PreselectedServiceId = serviceId,
                PreselectedStylistId = stylistId,
            }
        );
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(
        BookingInputModel input,
        CancellationToken cancellationToken
    )
    {
        if (!TimeOnly.TryParse(input.ScheduledTime, out TimeOnly time))
        {
            time = new TimeOnly(10, 0);
        }

        DateTime scheduledAt = input.ScheduledDate.Date.Add(time.ToTimeSpan());

        CreateAppointmentResult result = await mediator.SendAsync(
            new CreateAppointmentCommand(
                input.CustomerName,
                input.Phone,
                input.Email,
                input.HairServiceId,
                input.StylistId,
                scheduledAt,
                input.Notes
            ),
            cancellationToken
        );

        BookingFormDto form = await mediator.QueryAsync(new GetBookingFormQuery(), cancellationToken);

        if (!result.Success)
        {
            return View(
                new BookingViewModel
                {
                    Form = form,
                    PreselectedServiceId = input.HairServiceId,
                    PreselectedStylistId = input.StylistId,
                    ErrorMessage = result.ErrorMessage,
                }
            );
        }

        return View(
            new BookingViewModel
            {
                Form = form,
                SuccessMessage = localizer["Booking_Success"].Value,
            }
        );
    }
}
