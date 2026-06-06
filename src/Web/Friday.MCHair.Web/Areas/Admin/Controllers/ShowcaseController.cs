using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class ShowcaseController(ISalonRepository repository) : AdminControllerBase
{
    public async Task<IActionResult> Index(ShowcaseType type, CancellationToken cancellationToken)
    {
        if (!ShowcaseTypeInfo.AllTypes.Contains(type))
        {
            return RedirectToAction(nameof(Index), new { type = (int)ShowcaseType.Feedback });
        }

        ViewBag.Type = type;
        ViewData["Title"] = ShowcaseTypeLabels.GetLabel(type);
        return View(await repository.GetAllShowcaseAsync(type, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(
        ShowcaseType type,
        IFormFile imageFile,
        string? title,
        CancellationToken cancellationToken
    )
    {
        if (!ShowcaseTypeInfo.AllTypes.Contains(type))
        {
            TempData["Error"] = "Danh mục không hợp lệ.";
            return RedirectToAction(nameof(Index), new { type = (int)ShowcaseType.Feedback });
        }

        if (imageFile is null || imageFile.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn ảnh để tải lên.";
            return RedirectToAction(nameof(Index), new { type = (int)type });
        }

        try
        {
            IImageUploadService uploadService =
                HttpContext.RequestServices.GetRequiredService<IImageUploadService>();
            string imageUrl = await uploadService.SaveAsync(
                imageFile,
                ShowcaseTypeInfo.GetFolderSlug(type),
                cancellationToken
            );

            IReadOnlyList<ShowcaseItem> existing = await repository.GetAllShowcaseAsync(
                type,
                cancellationToken
            );
            int nextSort = existing.Count == 0 ? 1 : existing.Max(x => x.SortOrder) + 1;

            await repository.AddShowcaseItemAsync(
                new ShowcaseItem
                {
                    Title = string.IsNullOrWhiteSpace(title)
                        ? Path.GetFileNameWithoutExtension(imageFile.FileName)
                        : title.Trim(),
                    Type = type,
                    ImageUrl = imageUrl,
                    SortOrder = nextSort,
                    IsPublished = true,
                },
                cancellationToken
            );
            await CommitAsync(cancellationToken);
            TempData["Success"] = "Đã thêm ảnh.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { type = (int)type });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ShowcaseItem? item = await repository.GetShowcaseByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound();
        }

        IImageUploadService uploadService =
            HttpContext.RequestServices.GetRequiredService<IImageUploadService>();
        uploadService.TryDeleteResourceFile(item.ImageUrl);

        await repository.DeleteShowcaseItemAsync(item, cancellationToken);
        await CommitAsync(cancellationToken);
        TempData["Success"] = "Đã xóa ảnh.";
        return RedirectToAction(nameof(Index), new { type = (int)item.Type });
    }
}
