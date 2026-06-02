using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class GalleryController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Home");
}
