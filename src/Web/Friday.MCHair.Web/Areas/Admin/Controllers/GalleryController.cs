using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class GalleryController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(
        GalleryCategory? category,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<GalleryItem> items = await repository.GetAllGalleryAsync(cancellationToken);
        IReadOnlyList<GalleryItem> filtered =
            category.HasValue
                ? items.Where(x => x.Category == category.Value).ToList()
                : items;

        ViewBag.Category = category;
        return View(filtered);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(
        GalleryCategory category,
        IFormFile imageFile,
        string? title,
        CancellationToken cancellationToken
    )
    {
        if (!GalleryCategoryInfo.CollectionCategories.Contains(category))
        {
            TempData["Error"] = "Danh mục không hợp lệ.";
            return RedirectToAction(nameof(Index));
        }

        if (imageFile is null || imageFile.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn ảnh để tải lên.";
            return RedirectToAction(nameof(Index), new { category = (int?)category });
        }

        try
        {
            string folder = $"bo_suu_tap_{GalleryCategoryInfo.GetFolderSlug(category)}";
            IImageUploadService uploadService =
                HttpContext.RequestServices.GetRequiredService<IImageUploadService>();
            string imageUrl = await uploadService.SaveAsync(imageFile, folder, cancellationToken);

            IReadOnlyList<GalleryItem> existing = await repository.GetAllGalleryAsync(
                cancellationToken
            );
            int nextSort =
                existing.Count == 0 ? 1 : existing.Max(x => x.SortOrder) + 1;

            await repository.AddGalleryItemAsync(
                new GalleryItem
                {
                    Title = string.IsNullOrWhiteSpace(title)
                        ? Path.GetFileNameWithoutExtension(imageFile.FileName)
                        : title.Trim(),
                    Category = category,
                    ImageUrl = imageUrl,
                    SortOrder = nextSort,
                    IsPublished = true,
                },
                cancellationToken
            );
            await CommitAsync(cancellationToken);
            TempData["Success"] = "Đã thêm ảnh vào bộ sưu tập.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { category = (int?)category });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        GalleryItem? item = await repository.GetGalleryByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        IImageUploadService uploadService =
            HttpContext.RequestServices.GetRequiredService<IImageUploadService>();
        uploadService.TryDeleteLocalUpload(item.ImageUrl);

        await repository.DeleteGalleryItemAsync(item, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã xóa ảnh.";
        return RedirectToAction(nameof(Index), new { category = (int?)item.Category });
    }
}
