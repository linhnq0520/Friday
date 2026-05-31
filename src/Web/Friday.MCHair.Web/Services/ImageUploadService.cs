namespace Friday.MCHair.Web.Services;

public interface IImageUploadService
{
    Task<string> SaveAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

    void TryDeleteLocalUpload(string? imageUrl);
}

public sealed class ImageUploadService(IWebHostEnvironment environment) : IImageUploadService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<string> SaveAsync(
        IFormFile file,
        string folder,
        CancellationToken cancellationToken = default
    )
    {
        if (file.Length == 0)
        {
            throw new InvalidOperationException("File ảnh trống.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new InvalidOperationException("Ảnh không được lớn hơn 5MB.");
        }

        string extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Chỉ chấp nhận ảnh JPG, PNG, WEBP hoặc GIF.");
        }

        string safeFolder = string.Join(
            "_",
            folder.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)
        );
        string uploadDirectory = Path.Combine(environment.WebRootPath, "uploads", safeFolder);
        Directory.CreateDirectory(uploadDirectory);

        string fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        string physicalPath = Path.Combine(uploadDirectory, fileName);

        await using FileStream stream = File.Create(physicalPath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{safeFolder}/{fileName}";
    }

    public void TryDeleteLocalUpload(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        string normalized = imageUrl.Trim().Replace('\\', '/');
        if (!normalized.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string relativePath = normalized.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        string physicalPath = Path.GetFullPath(Path.Combine(environment.WebRootPath, relativePath));
        string uploadsRoot = Path.GetFullPath(Path.Combine(environment.WebRootPath, "uploads"));

        if (!physicalPath.StartsWith(uploadsRoot, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }
}
