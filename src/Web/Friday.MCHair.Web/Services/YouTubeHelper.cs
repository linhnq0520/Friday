using System;
using System.Text.RegularExpressions;

namespace Friday.MCHair.Web.Services;

public static class YouTubeHelper
{
    private static readonly Regex YouTubeRegex = new(
        @"(?:https?:\/\/)?(?:www\.|m\.)?(?:youtube\.com\/(?:watch\?(?:.*&)?v=|embed\/|v\/|shorts\/|live\/)|youtu\.be\/)([\w-]{11})",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex RawIdRegex = new(
        @"^[\w-]{11}$",
        RegexOptions.Compiled
    );

    /// <summary>
    /// Extracts the 11-character YouTube video ID from various YouTube URL formats.
    /// </summary>
    public static string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        string trimmed = url.Trim();

        // 1. Check if user provided just the 11-char video ID
        if (RawIdRegex.IsMatch(trimmed))
        {
            return trimmed;
        }

        // 2. Primary regex matching
        Match match = YouTubeRegex.Match(trimmed);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        // 3. Fallback: Parse Uri query string if present
        if (Uri.TryCreate(trimmed.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? trimmed : $"https://{trimmed}", UriKind.Absolute, out Uri? uri))
        {
            if (uri.Host.Contains("youtube.com", StringComparison.OrdinalIgnoreCase))
            {
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string? v = queryParams["v"];
                if (!string.IsNullOrWhiteSpace(v) && RawIdRegex.IsMatch(v.Trim()))
                {
                    return v.Trim();
                }

                // Check path segments (shorts/..., embed/..., live/...)
                string[] segments = uri.AbsolutePath.Trim('/').Split('/');
                if (segments.Length >= 2 && (segments[0].Equals("shorts", StringComparison.OrdinalIgnoreCase) ||
                                             segments[0].Equals("embed", StringComparison.OrdinalIgnoreCase) ||
                                             segments[0].Equals("live", StringComparison.OrdinalIgnoreCase) ||
                                             segments[0].Equals("v", StringComparison.OrdinalIgnoreCase)))
                {
                    if (RawIdRegex.IsMatch(segments[1]))
                    {
                        return segments[1];
                    }
                }
            }
            else if (uri.Host.Contains("youtu.be", StringComparison.OrdinalIgnoreCase))
            {
                string path = uri.AbsolutePath.Trim('/');
                if (RawIdRegex.IsMatch(path))
                {
                    return path;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Checks whether the provided string contains a valid YouTube video reference.
    /// </summary>
    public static bool IsYouTubeUrl(string? url) => !string.IsNullOrWhiteSpace(ExtractVideoId(url));

    /// <summary>
    /// Converts a YouTube video URL into a safe, privacy-enhanced embed URL (youtube-nocookie.com/embed/VIDEO_ID).
    /// </summary>
    public static string? ToEmbedUrl(string? url)
    {
        string? videoId = ExtractVideoId(url);
        return videoId is not null ? $"https://www.youtube-nocookie.com/embed/{videoId}" : null;
    }

    /// <summary>
    /// Converts any YouTube URL to the standard canonical watch URL (youtube.com/watch?v=VIDEO_ID).
    /// </summary>
    public static string? ToWatchUrl(string? url)
    {
        string? videoId = ExtractVideoId(url);
        return videoId is not null ? $"https://www.youtube.com/watch?v={videoId}" : null;
    }

    /// <summary>
    /// Returns the YouTube thumbnail image URL for a given video.
    /// Available qualities: maxresdefault, hqdefault, mqdefault, default.
    /// </summary>
    public static string? ToThumbnailUrl(string? url, string quality = "hqdefault")
    {
        string? videoId = ExtractVideoId(url);
        return videoId is not null ? $"https://img.youtube.com/vi/{videoId}/{quality}.jpg" : null;
    }
}
