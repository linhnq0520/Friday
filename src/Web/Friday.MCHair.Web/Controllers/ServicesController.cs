using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

[Route("dich-vu")]
[Route("Services")]
public sealed class ServicesController(
    IMediator mediator,
    ISalonRepository repository,
    IUiLocalizer localizer
) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<HairServiceDto> services = await mediator.QueryAsync(
            new GetServicesPageQuery(),
            cancellationToken
        );
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );
        IReadOnlyList<GalleryItem> gallery = await repository.GetPublishedGalleryAsync(
            null,
            cancellationToken
        );

        ViewData["Title"] = localizer["Meta_Services"].Value;
        ViewData["MetaDescription"] = CultureHelper.IsEnglish
            ? "Professional hair styling services at MC Hair Salon: Haircut, Color, Perm, Hair Extensions, Straightening, Damage Repair. Upfront pricing, 1:1 dedicated stylists."
            : "Dịch vụ làm tóc chuyên nghiệp tại MC Hair Salon: Cắt tóc, Uốn, Nhuộm, Nối tóc, Duỗi, Phục hồi tóc hư tổn. Báo giá rõ ràng, stylist 1:1 tận tâm tại TP.HCM.";

        return View(
            new ServicesHubViewModel
            {
                Services = services,
                Settings = settings,
                GalleryItems = gallery
                    .Take(6)
                    .Select(x => new GalleryItemDto(x.Id, x.Title, x.Category, x.ImageUrl))
                    .ToList(),
            }
        );
    }

    [HttpGet("{slug}")]
    [HttpGet("/{slug:regex(^(noi-toc|uon-toc|nhuom-toc|cat-toc-nu|duoi-toc|phuc-hoi-toc)$)}")]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return RedirectToAction(nameof(Index));
        }

        HairServiceDto? service = await mediator.QueryAsync(
            new GetServiceDetailBySlugQuery(slug),
            cancellationToken
        );

        if (service is null)
        {
            // Fallback: check if id
            if (int.TryParse(slug, out int id))
            {
                HairService? entity = await repository.GetServiceByIdAsync(id, cancellationToken);
                if (entity is not null && !string.IsNullOrWhiteSpace(entity.Slug))
                {
                    return RedirectToActionPermanent(nameof(Detail), new { slug = entity.Slug });
                }
            }
            return NotFound();
        }

        IReadOnlyList<HairServiceDto> allServices = await mediator.QueryAsync(
            new GetServicesPageQuery(),
            cancellationToken
        );
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );

        string serviceTitle = CultureHelper.IsEnglish
            ? $"{service.Name} in Ho Chi Minh City | MC Hair Salon"
            : $"{service.Name} TP.HCM: Báo Giá & Tư Vấn Chuẩn | MC Hair Salon";

        ViewData["Title"] = serviceTitle;
        ViewData["MetaDescription"] = service.ShortDescription ?? service.Description ?? service.Headline ?? serviceTitle;

        return View(
            "Detail",
            new ServiceDetailViewModel
            {
                Service = service,
                OtherServices = allServices.Where(x => x.Id != service.Id).ToList(),
                Settings = settings,
            }
        );
    }
}
