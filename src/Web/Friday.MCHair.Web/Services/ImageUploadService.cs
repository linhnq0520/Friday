namespace Friday.MCHair.Web.Services;

public interface IImageUploadService
{
    Task<string> SaveAsync(IFormFile file, string resourcesFolder, CancellationToken cancellationToken = default);

    void TryDeleteResourceFile(string? imageUrl);
}

public sealed class ImageUploadService(IWebHostEnvironment environment) : IImageUploadService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<string> SaveAsync(
        IFormFile file,
        string resourcesFolder,
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

        string relativeFolder = SanitizeResourceFolder(resourcesFolder);
        string directory = Path.Combine(
            environment.WebRootPath,
            "resources",
            relativeFolder.Replace('/', Path.DirectorySeparatorChar)
        );
        Directory.CreateDirectory(directory);

        string fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        string physicalPath = Path.Combine(directory, fileName);

        await using FileStream stream = File.Create(physicalPath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/resources/{relativeFolder}/{fileName}";
    }

    public void TryDeleteResourceFile(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        string normalized = imageUrl.Trim().Replace('\\', '/');
        if (!normalized.StartsWith("/resources/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string relativePath = normalized["/resources/".Length..];
        if (relativePath.Contains("..", StringComparison.Ordinal))
        {
            return;
        }

        string physicalPath = Path.GetFullPath(
            Path.Combine(
                environment.WebRootPath,
                "resources",
                relativePath.Replace('/', Path.DirectorySeparatorChar)
            )
        );
        string resourcesRoot = Path.GetFullPath(Path.Combine(environment.WebRootPath, "resources"));

        if (!physicalPath.StartsWith(resourcesRoot, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }

    private static string SanitizeResourceFolder(string folder)
    {
        IEnumerable<string> segments = folder
            .Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries)
            .Select(segment =>
                string.Join(
                    "_",
                    segment.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)
                )
            )
            .Where(segment => !string.IsNullOrWhiteSpace(segment));

        return string.Join("/", segments);
    }
}
