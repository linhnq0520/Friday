using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class WarrantyController(IWarrantyStore warrantyStore) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        WarrantyPageData page = await warrantyStore.GetAsync(cancellationToken);
        ViewData["Title"] = $"{page.Title} | MC Hair Salon";
        ViewData["MetaDescription"] = string.IsNullOrWhiteSpace(page.MetaDescription)
            ? CultureHelper.IsEnglish
                ? "MC Hair Salon warranty policy for hair services – quality commitment and client satisfaction."
                : "Chính sách bảo hành dịch vụ làm tóc tại MC Hair Salon – cam kết chất lượng và hài lòng khách hàng."
            : page.MetaDescription;
        return View(page);
    }
}
