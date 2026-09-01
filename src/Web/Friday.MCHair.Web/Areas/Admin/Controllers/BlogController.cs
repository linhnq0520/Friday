using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

public sealed class BlogController(
    ISalonRepository repository,
    IImageUploadService uploadService
) : AdminControllerBase
{
    public async Task<IActionResult> Index(
        [FromQuery] string? category,
        [FromQuery] string? search,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<BlogPost> posts = await repository.GetAllBlogPostsAsync(
            category,
            search,
            cancellationToken
        );
        IReadOnlyList<string> categories = await repository.GetDistinctCategoriesAsync(cancellationToken);

        ViewBag.SelectedCategory = category;
        ViewBag.SearchQuery = search;
        ViewBag.Categories = categories;

        return View(posts);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        BlogPost model = id.HasValue
            ? await repository.GetBlogPostByIdAsync(id.Value, cancellationToken) ?? new BlogPost()
            : new BlogPost
            {
                IsPublished = true,
                PublishedAt = DateTime.UtcNow,
                AuthorName = "MC Hair Team",
                Category = "Xu hướng tóc"
            };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        BlogPost model,
        IFormFile? imageFile,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(model.Title))
        {
            ModelState.AddModelError(nameof(model.Title), "Vui lòng nhập tiêu đề bài viết.");
            return View(model);
        }

        BlogPost? existing = model.Id > 0
            ? await repository.GetBlogPostByIdAsync(model.Id, cancellationToken)
            : null;

        // Auto-generate or sanitize slug
        if (string.IsNullOrWhiteSpace(model.Slug))
        {
            model.Slug = GenerateSlug(model.Title);
        }
        else
        {
            model.Slug = GenerateSlug(model.Slug);
        }

        // Check slug uniqueness
        BlogPost? postWithSameSlug = await repository.GetBlogPostBySlugAsync(model.Slug, cancellationToken);
        if (postWithSameSlug != null && postWithSameSlug.Id != model.Id)
        {
            model.Slug = $"{model.Slug}-{DateTime.UtcNow:MMddHHmm}";
        }

        try
        {
            model.ThumbnailUrl = await this.ResolveImageUrlAsync(
                imageFile,
                "blog",
                existing?.ThumbnailUrl,
                model.ThumbnailUrl,
                cancellationToken
            );
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        if (existing != null)
        {
            model.ViewCount = existing.ViewCount;
        }

        // Sanitize rich HTML content against XSS and SEO spam link hijacking
        model.Content = HtmlSecuritySanitizer.Sanitize(model.Content);

        await repository.AddBlogPostAsync(model, cancellationToken);
        await CommitAsync(cancellationToken);

        TempData["Success"] = model.Id == 0 || existing == null
            ? "Đã tạo bài viết mới thành công."
            : "Đã cập nhật bài viết thành công.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        BlogPost? post = await repository.GetBlogPostByIdAsync(id, cancellationToken);
        if (post != null)
        {
            DeleteAllPostImages(post);
            await repository.DeleteBlogPostAsync(post, cancellationToken);
            await CommitAsync(cancellationToken);
            TempData["Success"] = "Đã xóa bài viết và toàn bộ hình ảnh liên quan.";
        }

        return RedirectToAction(nameof(Index));
    }

    private void DeleteAllPostImages(BlogPost post)
    {
        if (!string.IsNullOrWhiteSpace(post.ThumbnailUrl))
        {
            uploadService.TryDeleteResourceFile(post.ThumbnailUrl);
        }

        if (!string.IsNullOrWhiteSpace(post.Content))
        {
            System.Text.RegularExpressions.MatchCollection matches =
                System.Text.RegularExpressions.Regex.Matches(
                    post.Content,
                    @"(/resources/[a-zA-Z0-9_\-/\.]+\.(?:jpg|jpeg|png|webp|gif))",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Success)
                {
                    uploadService.TryDeleteResourceFile(match.Value);
                }
            }
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadContentImage(
        IFormFile? upload,
        CancellationToken cancellationToken
    )
    {
        if (upload == null || upload.Length == 0)
        {
            return BadRequest(new { error = "Không tìm thấy file ảnh." });
        }

        try
        {
            string url = await uploadService.SaveAsync(upload, "blog/content", cancellationToken);
            return Json(new { url, uploaded = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    public sealed record InsertImageUrlRequest(string? Url, bool DownloadToSalon = true);

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> InsertImageByUrl(
        [FromBody] InsertImageUrlRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new { error = "Vui lòng nhập đường dẫn hình ảnh." });
        }

        string rawUrl = request.Url.Trim();
        string directUrl = ConvertToDirectImageUrl(rawUrl);

        if (!request.DownloadToSalon)
        {
            return Json(new { url = directUrl, direct = true });
        }

        try
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            var response = await httpClient.GetAsync(directUrl, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                // Fallback to direct URL if download from remote provider failed
                return Json(new { url = directUrl, fallback = true });
            }

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var ext = contentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                "image/gif" => ".gif",
                _ => ".jpg"
            };

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            if (bytes.Length > 20 * 1024 * 1024)
            {
                return BadRequest(new { error = "File ảnh vượt quá 20MB." });
            }

            using var memoryStream = new MemoryStream(bytes);
            var formFile = new FormFile(memoryStream, 0, bytes.Length, "upload", $"image{ext}")
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            string localUrl = await uploadService.SaveAsync(formFile, "blog/content", cancellationToken);
            return Json(new { url = localUrl, local = true });
        }
        catch (Exception)
        {
            // If download fails (network/restricted), fallback to direct URL
            return Json(new { url = directUrl, fallback = true });
        }
    }

    public static string ConvertToDirectImageUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;

        // Google Drive links:
        // https://drive.google.com/file/d/FILE_ID/view...
        // https://drive.google.com/open?id=FILE_ID
        // https://drive.google.com/uc?id=FILE_ID
        var gdMatch1 = Regex.Match(url, @"drive\.google\.com/file/d/([a-zA-Z0-9_-]+)");
        if (gdMatch1.Success)
        {
            string id = gdMatch1.Groups[1].Value;
            return $"https://lh3.googleusercontent.com/d/{id}";
        }

        var gdMatch2 = Regex.Match(url, @"drive\.google\.com/.*[?&]id=([a-zA-Z0-9_-]+)");
        if (gdMatch2.Success)
        {
            string id = gdMatch2.Groups[1].Value;
            return $"https://lh3.googleusercontent.com/d/{id}";
        }

        // Dropbox links: dl=0 -> raw=1
        if (url.Contains("dropbox.com") && url.Contains("dl=0"))
        {
            return url.Replace("dl=0", "raw=1");
        }

        return url;
    }

    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "bai-viet";
        }

        string normalized = text.Trim().ToLowerInvariant();

        // Remove Vietnamese accents
        normalized = Regex.Replace(normalized, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
        normalized = Regex.Replace(normalized, "[éèẻẽẹêếềểễệ]", "e");
        normalized = Regex.Replace(normalized, "[iíìỉĩị]", "i");
        normalized = Regex.Replace(normalized, "[óòỏõọôốồổỗộơớờởỡợ]", "o");
        normalized = Regex.Replace(normalized, "[úùủũụưứừửữự]", "u");
        normalized = Regex.Replace(normalized, "[ýỳỷỹỵ]", "y");
        normalized = Regex.Replace(normalized, "[đ]", "d");

        // Remove combining diacritical marks
        string formD = normalized.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new();
        foreach (char c in formD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        string withoutDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

        // Replace invalid chars with hyphen
        string slug = Regex.Replace(withoutDiacritics, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "bai-viet" : slug;
    }
}
