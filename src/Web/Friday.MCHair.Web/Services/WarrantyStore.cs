using System.Text.Json;
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

        if (
            settings.TryGetValue(SettingKey, out string? json)
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
                    return Normalize(data);
                }
            }
            catch (JsonException)
            {
                // Fall back to defaults if stored JSON is invalid.
            }
        }

        return WarrantyDefaults.Create();
    }

    public async Task SaveAsync(WarrantyPageData data, CancellationToken cancellationToken = default)
    {
        WarrantyPageData normalized = Normalize(data);
        string json = JsonSerializer.Serialize(normalized, JsonOptions);
        await repository.UpsertSettingAsync(SettingKey, json, cancellationToken);
    }

    public static WarrantyPageData Normalize(WarrantyPageData data)
    {
        data.Title = string.IsNullOrWhiteSpace(data.Title)
            ? WarrantyDefaults.Create().Title
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
