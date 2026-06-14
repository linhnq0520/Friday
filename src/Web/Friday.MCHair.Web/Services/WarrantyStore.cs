using System.Text.Json;
using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Domain.Repositories;

namespace Friday.MCHair.Web.Services;

public interface IWarrantyStore
{
    Task<WarrantyPageData> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(WarrantyPageData data, CancellationToken cancellationToken = default);
}

public sealed class WarrantyStore(ISalonRepository repository) : IWarrantyStore
{
    public const string SettingKey = "warranty_page_json";

    public const string SettingKeyEn = "warranty_page_json_en";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public async Task<WarrantyPageData> GetAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync(
            cancellationToken
        );

        string key = CultureHelper.IsEnglish ? SettingKeyEn : SettingKey;

        if (
            settings.TryGetValue(key, out string? json)
            && !string.IsNullOrWhiteSpace(json)
        )
        {
            try
            {
                WarrantyPageData? data = JsonSerializer.Deserialize<WarrantyPageData>(
                    json,
                    JsonOptions
                );
                if (data is not null)
                {
                    return Normalize(data, CultureHelper.IsEnglish);
                }
            }
            catch (JsonException)
            {
                // Fall back to defaults if stored JSON is invalid.
            }
        }

        return CultureHelper.IsEnglish ? WarrantyDefaultsEn.Create() : WarrantyDefaults.Create();
    }

    public async Task SaveAsync(WarrantyPageData data, CancellationToken cancellationToken = default)
    {
        WarrantyPageData normalized = Normalize(data, CultureHelper.IsEnglish);
        string json = JsonSerializer.Serialize(normalized, JsonOptions);
        string key = CultureHelper.IsEnglish ? SettingKeyEn : SettingKey;
        await repository.UpsertSettingAsync(key, json, cancellationToken);
    }

    public static WarrantyPageData Normalize(WarrantyPageData data, bool isEnglish)
    {
        WarrantyPageData defaults = isEnglish
            ? WarrantyDefaultsEn.Create()
            : WarrantyDefaults.Create();

        data.Title = string.IsNullOrWhiteSpace(data.Title)
            ? defaults.Title
            : data.Title.Trim();
        data.Lead = data.Lead?.Trim() ?? string.Empty;
        data.MetaDescription = data.MetaDescription?.Trim() ?? string.Empty;
        data.Sections = data
            .Sections.Where(s =>
                !string.IsNullOrWhiteSpace(s.Title) || !string.IsNullOrWhiteSpace(s.Body)
            )
            .Select(s => new WarrantySectionData
            {
                Title = s.Title.Trim(),
                Format = string.Equals(s.Format, "list", StringComparison.OrdinalIgnoreCase)
                    ? "list"
                    : "paragraph",
                Body = s.Body.Trim(),
            })
            .ToList();

        return data;
    }
}
