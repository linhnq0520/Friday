using System.Net.Http.Headers;
using System.Net.Http.Json;
using Friday.AdminPortal.Models;

namespace Friday.AdminPortal.Services;

public sealed class AdminApiClient(HttpClient httpClient)
{
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public async Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponse = await httpClient.PostAsJsonAsync(
            "/api/auth/login",
            request,
            cancellationToken
        );
        httpResponse.EnsureSuccessStatusCode();
        ApiResponse<LoginResponse>? response = await httpResponse.Content.ReadFromJsonAsync<
            ApiResponse<LoginResponse>
        >(cancellationToken);
        if (response?.Data is null)
        {
            throw new InvalidOperationException("Login failed.");
        }

        AccessToken = response.Data.AccessToken;
        RefreshToken = response.Data.RefreshToken;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            AccessToken
        );
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(RefreshToken))
        {
            return;
        }

        await httpClient.PostAsJsonAsync(
            "/api/auth/logout",
            new LogoutRequest(RefreshToken),
            cancellationToken
        );
        AccessToken = null;
        RefreshToken = null;
        httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(
        CancellationToken cancellationToken = default
    ) => await GetListAsync<UserDto>("/api/admin/users", cancellationToken);

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(
        CancellationToken cancellationToken = default
    ) => await GetListAsync<RoleDto>("/api/admin/roles", cancellationToken);

    public async Task<IReadOnlyList<RightDto>> GetRightsAsync(
        CancellationToken cancellationToken = default
    ) => await GetListAsync<RightDto>("/api/admin/rights", cancellationToken);

    public async Task CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/api/admin/users",
            request,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            "/api/admin/roles",
            request,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task AssignRoleToUserAsync(
        int userId,
        int roleId,
        CancellationToken cancellationToken = default
    )
    {
        HttpResponseMessage response = await httpClient.PostAsync(
            $"/api/admin/users/{userId}/roles/{roleId}",
            null,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task GrantRightsToRoleAsync(
        int roleId,
        int[] rightIds,
        CancellationToken cancellationToken = default
    )
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync(
            $"/api/admin/roles/{roleId}/rights",
            rightIds,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    public async Task LockUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await httpClient.PostAsync(
            $"/api/admin/users/{userId}/lock",
            null,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
    }

    private async Task<IReadOnlyList<T>> GetListAsync<T>(
        string path,
        CancellationToken cancellationToken
    ) where T : class
    {
        ApiResponse<List<T>>? response = await httpClient.GetFromJsonAsync<ApiResponse<List<T>>>(
            path,
            cancellationToken
        );
        return response?.Data ?? [];
    }
}
