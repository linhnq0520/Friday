using Friday.MCHair.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public static class AdminImageUploadExtensions
{
    public static async Task<string?> ResolveImageUrlAsync(
        this Controller controller,
        IFormFile? imageFile,
        string resourcesFolder,
        string? currentImageUrl,
        string? submittedImageUrl,
        CancellationToken cancellationToken
    )
    {
        if (imageFile is not null && imageFile.Length > 0)
        {
            IImageUploadService uploadService =
                controller.HttpContext.RequestServices.GetRequiredService<IImageUploadService>();

            string newUrl = await uploadService.SaveAsync(imageFile, resourcesFolder, cancellationToken);

            if (
                !string.IsNullOrWhiteSpace(currentImageUrl)
                && !currentImageUrl.Equals(newUrl, StringComparison.OrdinalIgnoreCase)
            )
            {
                uploadService.TryDeleteResourceFile(currentImageUrl);
            }

            return newUrl;
        }

        if (!string.IsNullOrWhiteSpace(submittedImageUrl))
        {
            string trimmed = submittedImageUrl.Trim();
            if (
                !string.IsNullOrWhiteSpace(currentImageUrl)
                && !currentImageUrl.Equals(trimmed, StringComparison.OrdinalIgnoreCase)
            )
            {
                IImageUploadService uploadService =
                    controller.HttpContext.RequestServices.GetRequiredService<IImageUploadService>();
                uploadService.TryDeleteResourceFile(currentImageUrl);
            }

            return trimmed;
        }

        return currentImageUrl;
    }
}
