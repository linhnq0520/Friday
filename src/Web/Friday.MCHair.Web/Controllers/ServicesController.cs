using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class ServicesController(
    IMediator mediator,
    ISalonRepository repository,
    IPriceListStore priceListStore
) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<HairServiceDto> services = await mediator.QueryAsync(
            new GetServicesPageQuery(),
            cancellationToken
        );
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        PriceListData priceList = await priceListStore.GetAsync(cancellationToken);
        ViewData["Title"] = $"Dịch vụ & bảng giá | {settings.GetValueOrDefault("site_name", SeoDefaults.SiteName)}";
        ViewData["MetaDescription"] =
            "Bảng giá cắt tóc, nhuộm, uốn, duỗi, nối tóc và phục hồi tại MC Hair Salon. Hotline 0988305371.";
        return View(
            new ServicesIndexViewModel
            {
                Services = services,
                Settings = settings,
                PriceList = priceList,
            }
        );
    }
}
