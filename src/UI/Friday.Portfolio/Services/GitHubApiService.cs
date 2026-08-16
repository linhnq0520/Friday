using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace Friday.Portfolio.Services;

public sealed class GitHubApiService(HttpClient http, IJSRuntime js)
{
    private const string StorageKey = "ql_github_pat";
    public const string DefaultOwner = "linhnq0520";
    public const string DefaultRepo = "Friday";
    public const string TargetFilePath = "src/UI/Friday.Portfolio/wwwroot/data/profile.json";

    public const string DefaultBranch = "feature/quoclinh-web";

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, token.Trim());
        }
        catch
        {
            // fallback
        }
    }

    public async Task ClearTokenAsync()
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch
        {
            // fallback
        }
    }

    public async Task<(
        bool Success,
        string? CommitUrl,
        string? ErrorMessage
    )> CommitProfileJsonAsync(
        string content,
        string token,
        string owner = DefaultOwner,
        string repo = DefaultRepo,
        string path = TargetFilePath,
        string branch = DefaultBranch
    )
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return (false, null, "GitHub Personal Access Token (PAT) is required.");
        }

        try
        {
            var apiUrl = $"https://api.github.com/repos/{owner}/{repo}/contents/{path}";

            // Step 1: Get current file SHA if it exists on the specified branch
            string? fileSha = null;
            var getUrl = $"{apiUrl}?ref={Uri.EscapeDataString(branch)}";
            using (var getReq = new HttpRequestMessage(HttpMethod.Get, getUrl))
            {
                getReq.Headers.UserAgent.ParseAdd("Friday-Portfolio-CMS");
                getReq.Headers.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    token.Trim()
                );
                getReq.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.github+json")
                );

                var getResp = await http.SendAsync(getReq);
                if (getResp.IsSuccessStatusCode)
                {
                    var fileInfo = await getResp.Content.ReadFromJsonAsync<GitHubContentResponse>();
                    fileSha = fileInfo?.Sha;
                }
            }

            // Step 2: Base64 encode the new JSON content
            var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

            // Step 3: Put updated content on target branch
            var payload = new GitHubPutPayload
            {
                Message =
                    $"docs(cms): update profile.json via Admin Editor [{DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC]",
                Content = base64Content,
                Sha = fileSha,
                Branch = branch,
            };

            using var putReq = new HttpRequestMessage(HttpMethod.Put, apiUrl);
            putReq.Headers.UserAgent.ParseAdd("Friday-Portfolio-CMS");
            putReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim());
            putReq.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github+json")
            );
            putReq.Content = JsonContent.Create(payload);

            var putResp = await http.SendAsync(putReq);

            if (putResp.IsSuccessStatusCode)
            {
                var result = await putResp.Content.ReadFromJsonAsync<GitHubCommitResponse>();
                var commitHtmlUrl =
                    result?.Commit?.HtmlUrl ?? $"https://github.com/{owner}/{repo}/commits/{branch}";
                return (true, commitHtmlUrl, null);
            }
            else
            {
                var errorBody = await putResp.Content.ReadAsStringAsync();
                return (false, null, $"GitHub API Error ({putResp.StatusCode}): {errorBody}");
            }
        }
        catch (Exception ex)
        {
            return (false, null, $"Exception: {ex.Message}");
        }
    }

    private sealed class GitHubContentResponse
    {
        [JsonPropertyName("sha")]
        public string? Sha { get; set; }
    }

    private sealed class GitHubPutPayload
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("sha")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Sha { get; set; }

        [JsonPropertyName("branch")]
        public string Branch { get; set; } = "main";
    }

    private sealed class GitHubCommitResponse
    {
        [JsonPropertyName("commit")]
        public GitHubCommitInfo? Commit { get; set; }
    }

    private sealed class GitHubCommitInfo
    {
        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }
    }
}
