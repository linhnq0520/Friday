using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Friday.Modules.Salon.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class ServicesController(
    ISalonRepository repository,
    IPriceListStore priceListStore
) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<HairService> items = await repository.GetAllServicesAsync(cancellationToken);
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        HairService model =
            id.HasValue
                ? await repository.GetServiceByIdAsync(id.Value, cancellationToken)
                    ?? new HairService()
                : new HairService { IsActive = true, RatingDisplay = 5 };

        PriceListData masterPriceList = await priceListStore.GetAsync(cancellationToken);
        ViewBag.MasterPriceGroups = masterPriceList.Groups;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetMasterPriceList(CancellationToken cancellationToken)
    {
        PriceListData masterPriceList = await priceListStore.GetAsync(cancellationToken);
        return Json(new
        {
            success = true,
            groups = masterPriceList.Groups.Select(g => new
            {
                title = g.Title,
                items = g.Items.Select(i => new
                {
                    name = i.Name,
                    price = i.Price
                })
            })
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        HairService model,
        IFormFile? imageFile,
        IFormFile? heroImageFile,
        IFormFile? beforeImageFile,
        IFormFile? afterImageFile,
        CancellationToken cancellationToken
    )
    {
        HairService? existing =
            model.Id > 0 ? await repository.GetServiceByIdAsync(model.Id, cancellationToken) : null;

        // Auto generate slug if empty
        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = SalonDataSeeder.GenerateSlug(model.Name);
        }
        else
        {
            model.Slug = SalonDataSeeder.GenerateSlug(model.Slug);
        }

        try
        {
            model.ImageUrl = await this.ResolveImageUrlAsync(
                imageFile,
                "dich_vu",
                existing?.ImageUrl,
                model.ImageUrl,
                cancellationToken
            );

            model.HeroImageUrl = await this.ResolveImageUrlAsync(
                heroImageFile,
                "dich_vu",
                existing?.HeroImageUrl,
                model.HeroImageUrl,
                cancellationToken
            );

            model.BeforeImageUrl = await this.ResolveImageUrlAsync(
                beforeImageFile,
                "before_after",
                existing?.BeforeImageUrl,
                model.BeforeImageUrl,
                cancellationToken
            );

            model.AfterImageUrl = await this.ResolveImageUrlAsync(
                afterImageFile,
                "before_after",
                existing?.AfterImageUrl,
                model.AfterImageUrl,
                cancellationToken
            );
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        await repository.AddServiceAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã lưu dịch vụ thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        HairService? item = await repository.GetServiceByIdAsync(id, cancellationToken);
        if (item is not null)
        {
            IImageUploadService uploadService =
                HttpContext.RequestServices.GetRequiredService<IImageUploadService>();
            uploadService.TryDeleteResourceFile(item.ImageUrl);
            uploadService.TryDeleteResourceFile(item.HeroImageUrl);

            await repository.DeleteServiceAsync(item, cancellationToken);
            await CommitAsync(cancellationToken);
            TempData["Success"] = "Đã xóa dịch vụ và ảnh liên quan.";
        }

        return RedirectToAction(nameof(Index));
    }
}
