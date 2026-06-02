using System.Text.Json;
using Friday.MCHair.Web.Models;
using Friday.Modules.Salon.Domain.Repositories;

namespace Friday.MCHair.Web.Services;

public interface IPriceListStore
{
    Task<PriceListData> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(PriceListData data, CancellationToken cancellationToken = default);
}

public sealed class PriceListStore(ISalonRepository repository) : IPriceListStore
{
    public const string SettingKey = "price_list_json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public async Task<PriceListData> GetAsync(CancellationToken cancellationToken = default)
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
                PriceListData? data = JsonSerializer.Deserialize<PriceListData>(json, JsonOptions);
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

        return PriceListDefaults.Create();
    }

    public async Task SaveAsync(PriceListData data, CancellationToken cancellationToken = default)
    {
        PriceListData normalized = Normalize(data);
        string json = JsonSerializer.Serialize(normalized, JsonOptions);
        await repository.UpsertSettingAsync(SettingKey, json, cancellationToken);
    }

    public static PriceListData Normalize(PriceListData data)
    {
        data.LengthGuide = data
            .LengthGuide.Where(x =>
                !string.IsNullOrWhiteSpace(x.Size) || !string.IsNullOrWhiteSpace(x.Description)
            )
            .ToList();

        List<PriceGroupData> groups = [];
        foreach (
            IGrouping<int, PriceGroupData> columnGroups in data
                .Groups.Where(g => !string.IsNullOrWhiteSpace(g.Title))
                .GroupBy(g => g.ColumnIndex)
                .OrderBy(g => g.Key)
        )
        {
            int sortOrder = 0;
            foreach (PriceGroupData group in columnGroups.OrderBy(g => g.SortOrder))
            {
                group.Items = group
                    .Items.Where(i =>
                        !string.IsNullOrWhiteSpace(i.Name) || !string.IsNullOrWhiteSpace(i.Price)
                    )
                    .ToList();
                group.SortOrder = sortOrder++;
                groups.Add(group);
            }
        }

        data.Groups = groups;
        return data;
    }
}
