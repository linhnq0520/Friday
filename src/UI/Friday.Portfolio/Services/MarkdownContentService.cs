using Markdig;
using Microsoft.AspNetCore.Components;

namespace Friday.Portfolio.Services;

/// <summary>
/// Loads Markdown from wwwroot and renders HTML (same HttpClient DI style as Friday.AdminPortal).
/// </summary>
public sealed class MarkdownContentService(HttpClient http)
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    private readonly Dictionary<string, MarkupString> _cache = new(StringComparer.OrdinalIgnoreCase);

    public async Task<MarkupString> GetHtmlAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(relativePath, out var cached))
        {
            return cached;
        }

        try
        {
            var markdown = await http.GetStringAsync(relativePath, cancellationToken);
            var html = Markdown.ToHtml(markdown, Pipeline);
            var markup = new MarkupString(html);
            _cache[relativePath] = markup;
            return markup;
        }
        catch (HttpRequestException)
        {
            return new MarkupString("<p class=\"content-missing\">Content is not available yet.</p>");
        }
    }
}
