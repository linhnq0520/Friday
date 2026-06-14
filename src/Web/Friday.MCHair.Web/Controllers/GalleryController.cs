using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Enums;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class GalleryController(
    IMediator mediator,
    IUiLocalizer localizer
) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<GalleryCollectionDto> collections = await mediator.QueryAsync(
            new GetGalleryCollectionsQuery(),
            cancellationToken
        );
        ViewData["Title"] = localizer["Meta_Gallery"].Value;
        ViewData["MetaDescription"] = CultureHelper.IsEnglish
            ? "Explore MC Hair Salon gallery: fashion color, trending styles, repair and extensions."
            : "Khám phá bộ sưu tập mẫu tóc hot: màu thời trang, kiểu tóc thịnh hành, phục hồi hư tổn và nối tóc tại MC Hair Salon.";
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
        string label = GalleryCategoryLabels.GetLabel(galleryCategory);
        ViewData["Title"] = $"{label} | MC Hair Salon";
        ViewData["MetaDescription"] = CultureHelper.IsEnglish
            ? $"Browse the {label} collection at MC Hair Salon."
            : $"Xem bộ sưu tập {label} tại MC Hair Salon.";
        return View(items);
    }
}
