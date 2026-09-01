using System.Text.RegularExpressions;

namespace Friday.MCHair.Web.Services;

public static partial class HtmlSecuritySanitizer
{
    // Strip dangerous tags completely
    private static readonly Regex DangerousTagsRegex = new(
        @"<\s*(script|style|object|embed|applet|meta|form|svg|canvas|base)[^>]*>[\s\S]*?<\s*/\s*\1\s*>|<\s*(script|style|object|embed|applet|meta|form|svg|canvas|base)[^>]*/>|<\s*(script|style|object|embed|applet|meta|form|svg|canvas|base)[^>]*>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // Strip inline JS event handlers like onload=, onclick=, onerror=, etc.
    private static readonly Regex InlineEventsRegex = new(
        @"\s+on\w+\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // Strip javascript: pseudo protocols
    private static readonly Regex JavascriptProtocolRegex = new(
        @"href\s*=\s*(?:""javascript:[^""]*""|'javascript:[^']*')",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // Filter iframes: Only allow iframes pointing to YouTube embeds
    private static readonly Regex IframeRegex = new(
        @"<iframe\b([^>]*)src\s*=\s*[""']([^""']*)[""']([^>]*)>[\s\S]*?</iframe>|<iframe\b([^>]*)src\s*=\s*[""']([^""']*)[""']([^>]*)/>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    // Match external anchor tags to enforce nofollow & noopener
    private static readonly Regex AnchorRegex = new(
        @"<a\b(?<before>[^>]*?)href\s*=\s*[""'](?<url>https?://[^""']+)[""'](?<after>[^>]*)>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    /// <summary>
    /// Sanitizes HTML content: removes scripts, unauthorized iframes, event handlers, and enforces rel="nofollow noopener noreferrer" on external links.
    /// </summary>
    public static string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return string.Empty;
        }

        // 1. Remove dangerous executable tags
        string cleaned = DangerousTagsRegex.Replace(html, string.Empty);

        // 2. Remove inline event handlers (onerror, onload, onclick, etc.)
        cleaned = InlineEventsRegex.Replace(cleaned, string.Empty);

        // 3. Remove javascript: links
        cleaned = JavascriptProtocolRegex.Replace(cleaned, @"href=""#""");

        // 4. Validate iframes: keep only YouTube embeds, strip any other iframes
        cleaned = IframeRegex.Replace(cleaned, match =>
        {
            string src = match.Groups[2].Success && !string.IsNullOrEmpty(match.Groups[2].Value)
                ? match.Groups[2].Value
                : match.Groups[5].Value;

            if (src.Contains("youtube.com/embed/", StringComparison.OrdinalIgnoreCase) ||
                src.Contains("youtube-nocookie.com/embed/", StringComparison.OrdinalIgnoreCase))
            {
                return $@"<div class=""video-responsive""><iframe src=""{src}"" frameborder=""0"" allow=""accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"" allowfullscreen loading=""lazy""></iframe></div>";
            }

            return string.Empty;
        });

        // 5. Enforce rel="nofollow noopener noreferrer" and target="_blank" on external links to prevent SEO link hijacking
        cleaned = AnchorRegex.Replace(cleaned, match =>
        {
            string before = match.Groups["before"].Value;
            string url = match.Groups["url"].Value;
            string after = match.Groups["after"].Value;

            // Strip existing rel & target attributes to avoid duplicate/conflicting attributes
            before = Regex.Replace(before, @"\s*(rel|target)\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+)", string.Empty, RegexOptions.IgnoreCase);
            after = Regex.Replace(after, @"\s*(rel|target)\s*=\s*(?:""[^""]*""|'[^']*'|[^\s>]+)", string.Empty, RegexOptions.IgnoreCase);

            return $@"<a {before.Trim()} href=""{url}"" target=""_blank"" rel=""nofollow noopener noreferrer"" {after.Trim()}>".Replace("  ", " ");
        });

        return cleaned.Trim();
    }
}
