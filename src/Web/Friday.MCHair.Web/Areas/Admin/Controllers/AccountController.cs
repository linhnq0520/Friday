using System.Security.Claims;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Enums;
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
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

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

        string roleStr = result.Role?.ToString() ?? nameof(AdminRole.Admin);

        List<Claim> claims =
        [
            new(ClaimTypes.Name, username),
            new(ClaimTypes.GivenName, result.DisplayName ?? username),
            new(ClaimTypes.Role, roleStr),
            new("display_name", result.DisplayName ?? username),
            new("role", roleStr),
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
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        string currentPassword,
        string newPassword,
        string confirmPassword,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            ViewBag.Error = "Vui lòng nhập đầy đủ thông tin mật khẩu.";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Mật khẩu xác nhận không khớp với mật khẩu mới.";
            return View();
        }

        if (newPassword.Length < 6)
        {
            ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự.";
            return View();
        }

        string username = User.Identity?.Name ?? string.Empty;
        ChangePasswordResult result = await mediator.QueryAsync(
            new ChangePasswordCommand(username, currentPassword, newPassword),
            cancellationToken
        );

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            return View();
        }

        Friday.BuildingBlocks.Application.Abstractions.IUnitOfWork unitOfWork =
            HttpContext.RequestServices.GetRequiredService<Friday.BuildingBlocks.Application.Abstractions.IUnitOfWork>();
        await unitOfWork.CommitAsync(cancellationToken);

        TempData["Success"] = "Đổi mật khẩu thành công!";
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
