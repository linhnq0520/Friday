using System.Security.Claims;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

[Area("Admin")]
public sealed class AccountController(IMediator mediator) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string? returnUrl,
        CancellationToken cancellationToken
    )
    {
        AdminLoginResult result = await mediator.QueryAsync(
            new AdminLoginCommand(username, password),
            cancellationToken
        );

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        List<Claim> claims =
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.GivenName, result.DisplayName ?? username),
            new("display_name", result.DisplayName ?? username),
        ];

        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
