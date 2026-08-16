using Microsoft.JSInterop;

namespace Friday.Portfolio.Services;

public sealed class AdminAuthService(IJSRuntime js)
{
    private const string StorageKey = "ql_admin_auth_token";
    // Mật khẩu mặc định quản trị (bạn có thể thay đổi mật khẩu này bất cứ lúc nào)
    public const string DefaultPasscode = "quoclinh2026";

    private bool? _isAuthenticated;

    public async Task<bool> IsAuthenticatedAsync()
    {
        if (_isAuthenticated.HasValue)
        {
            return _isAuthenticated.Value;
        }

        try
        {
            var savedToken = await js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            _isAuthenticated = savedToken == DefaultPasscode;
        }
        catch
        {
            _isAuthenticated = false;
        }

        return _isAuthenticated.Value;
    }

    public async Task<bool> LoginAsync(string password)
    {
        if (password == DefaultPasscode)
        {
            _isAuthenticated = true;
            try
            {
                await js.InvokeVoidAsync("localStorage.setItem", StorageKey, DefaultPasscode);
            }
            catch
            {
                // LocalStorage fallback
            }
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        _isAuthenticated = false;
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch
        {
            // LocalStorage fallback
        }
    }
}
