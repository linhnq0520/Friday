using System.Net.Http.Json;
using System.Text.Json;
using Friday.Portfolio.Models;

namespace Friday.Portfolio.Services;

public sealed class ProfileService(HttpClient http)
{
    private ProfileModel? _cachedProfile;

    public async Task<ProfileModel> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedProfile != null)
        {
            return _cachedProfile;
        }

        try
        {
            var data = await http.GetFromJsonAsync<ProfileModel>("data/profile.json", cancellationToken);
            _cachedProfile = data ?? new ProfileModel();
        }
        catch
        {
            _cachedProfile = new ProfileModel();
        }

        return _cachedProfile;
    }

    public void UpdateProfileInMemory(ProfileModel updated)
    {
        _cachedProfile = updated;
    }

    public string ExportToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(_cachedProfile ?? new ProfileModel(), options);
    }
}
