using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
public sealed class UsersController(IMediator mediator, ISalonRepository repository) : AdminControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        IReadOnlyList<AdminUserDto> users = await mediator.QueryAsync(
            new GetAllAdminUsersQuery(),
            cancellationToken
        );

        IReadOnlyList<Stylist> stylists = await repository.GetAllStylistsAsync(cancellationToken);
        ViewBag.Stylists = stylists.ToDictionary(s => s.Id, s => s.Name);

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        IReadOnlyList<Stylist> stylists = await repository.GetActiveStylistsAsync(cancellationToken);
        ViewBag.Stylists = stylists;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string username,
        string displayName,
        string password,
        AdminRole role,
        int? stylistId,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Stylist> stylists = await repository.GetActiveStylistsAsync(cancellationToken);
        ViewBag.Stylists = stylists;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu.";
            return View();
        }

        CreateAdminUserResult result = await mediator.QueryAsync(
            new CreateAdminUserCommand(username, displayName, password, role, stylistId),
            cancellationToken
        );

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            return View();
        }

        await CommitAsync(cancellationToken);

        TempData["Success"] = $"Đã tạo tài khoản '{username}' thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        AdminUserDto? user = await mediator.QueryAsync(
            new GetAdminUserByIdQuery(id),
            cancellationToken
        );

        if (user is null)
        {
            return NotFound();
        }

        IReadOnlyList<Stylist> stylists = await repository.GetActiveStylistsAsync(cancellationToken);
        ViewBag.Stylists = stylists;
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        string displayName,
        AdminRole role,
        bool isActive,
        int? stylistId,
        string? newPassword,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Stylist> stylists = await repository.GetActiveStylistsAsync(cancellationToken);
        ViewBag.Stylists = stylists;

        UpdateAdminUserResult result = await mediator.QueryAsync(
            new UpdateAdminUserCommand(id, displayName, role, isActive, stylistId, newPassword),
            cancellationToken
        );

        if (!result.Success)
        {
            ViewBag.Error = result.ErrorMessage;
            AdminUserDto? user = await mediator.QueryAsync(new GetAdminUserByIdQuery(id), cancellationToken);
            return View(user);
        }

        await CommitAsync(cancellationToken);

        TempData["Success"] = "Cập nhật thông tin tài khoản thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        AdminUserDto? user = await mediator.QueryAsync(new GetAdminUserByIdQuery(id), cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, IFormCollection? form, CancellationToken cancellationToken)
    {
        string currentAdmin = User.Identity?.Name ?? string.Empty;
        UpdateAdminUserResult result = await mediator.QueryAsync(
            new DeleteAdminUserCommand(id, currentAdmin),
            cancellationToken
        );

        if (!result.Success)
        {
            TempData["Error"] = result.ErrorMessage;
        }
        else
        {
            await CommitAsync(cancellationToken);
            TempData["Success"] = "Đã xóa tài khoản thành công!";
        }

        return RedirectToAction(nameof(Index));
    }
}
