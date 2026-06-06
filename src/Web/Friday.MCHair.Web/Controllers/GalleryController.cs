using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Enums;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class GalleryController(IMediator mediator) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<GalleryCollectionDto> collections = await mediator.QueryAsync(
            new GetGalleryCollectionsQuery(),
            cancellationToken
        );
        ViewData["Title"] = "Bộ sưu tập | MC Hair Salon";
        ViewData["MetaDescription"] =
            "Khám phá bộ sưu tập mẫu tóc hot: màu thời trang, kiểu tóc thịnh hành, phục hồi hư tổn và nối tóc tại MC Hair Salon.";
        return View(collections);
    }

    public async Task<IActionResult> Category(int category, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(GalleryCategory), category))
        {
            return NotFound();
        }

        var galleryCategory = (GalleryCategory)category;
        if (!GalleryCategoryInfo.CollectionCategories.Contains(galleryCategory))
        {
            return NotFound();
        }

        IReadOnlyList<GalleryItemDto> items = await mediator.QueryAsync(
            new GetGalleryPageQuery(galleryCategory),
            cancellationToken
        );
        ViewBag.Category = galleryCategory;
        ViewData["Title"] = $"{GalleryCategoryLabels.GetLabel(galleryCategory)} | MC Hair Salon";
        ViewData["MetaDescription"] =
            $"Xem bộ sưu tập {GalleryCategoryLabels.GetLabel(galleryCategory)} tại MC Hair Salon.";
        return View(items);
    }
}
