using Microsoft.JSInterop;

namespace Friday.Portfolio.Services;

public sealed class LanguageService(IJSRuntime js)
{
    private const string StorageKey = "ql_language_pref";
    public string CurrentLanguage { get; private set; } = "vi";

    public event Action? OnLanguageChanged;

    public async Task InitializeAsync()
    {
        try
        {
            var saved = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(saved) && (saved == "vi" || saved == "en"))
            {
                CurrentLanguage = saved;
            }
        }
        catch
        {
            CurrentLanguage = "vi";
        }
    }

    public async Task SetLanguageAsync(string lang)
    {
        if (lang != "vi" && lang != "en") return;
        if (CurrentLanguage == lang) return;

        CurrentLanguage = lang;
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, lang);
        }
        catch
        {
            // fallback
        }

        OnLanguageChanged?.Invoke();
    }

    public bool IsVietnamese => CurrentLanguage == "vi";
    public bool IsEnglish => CurrentLanguage == "en";

    public string Text(string vi, string en) => IsVietnamese ? vi : en;
}
