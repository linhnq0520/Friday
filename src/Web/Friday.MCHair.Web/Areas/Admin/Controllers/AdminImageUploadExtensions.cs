using Friday.MCHair.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public static class AdminImageUploadExtensions
{
    public static async Task<string?> ResolveImageUrlAsync(
        this Controller controller,
        IFormFile? imageFile,
        string uploadFolder,
        string? currentImageUrl,
        string? submittedImageUrl,
        CancellationToken cancellationToken
    )
    {
        if (imageFile is not null && imageFile.Length > 0)
        {
            IImageUploadService uploadService =
                controller.HttpContext.RequestServices.GetRequiredService<IImageUploadService>();

            string newUrl = await uploadService.SaveAsync(imageFile, uploadFolder, cancellationToken);

            if (
                !string.IsNullOrWhiteSpace(currentImageUrl)
                && !currentImageUrl.Equals(newUrl, StringComparison.OrdinalIgnoreCase)
            )
            {
                uploadService.TryDeleteLocalUpload(currentImageUrl);
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
                uploadService.TryDeleteLocalUpload(currentImageUrl);
            }

            return trimmed;
        }

        return currentImageUrl;
    }
}
