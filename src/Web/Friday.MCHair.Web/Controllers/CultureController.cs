using Friday.MCHair.Web.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class CultureController : Controller
{
    [HttpGet("/culture/set")]
    public IActionResult Set(string culture, string? returnUrl)
    {
        if (!CultureConstants.SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase))
        {
            culture = CultureConstants.Vietnamese;
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
            }
        );

        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        {
            returnUrl = "/";
        }

        return LocalRedirect(returnUrl);
    }
}
