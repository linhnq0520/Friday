using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class SitemapController : Controller
{
    [HttpGet("/sitemap.xml")]
    public IActionResult Index()
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XElement urlset = new(
            ns + "urlset",
            CreateUrl(ns, baseUrl + "/", "daily", "1.0"),
            CreateUrl(ns, baseUrl + "/About", "monthly", "0.9"),
            CreateUrl(ns, baseUrl + "/Services", "weekly", "0.9"),
            CreateUrl(ns, baseUrl + "/News", "weekly", "0.8"),
            CreateUrl(ns, baseUrl + "/Warranty", "monthly", "0.7"),
            CreateUrl(ns, baseUrl + "/Booking", "monthly", "0.9")
        );

        return Content(urlset.ToString(), "application/xml", Encoding.UTF8);
    }

    private static XElement CreateUrl(XNamespace ns, string loc, string changefreq, string priority) =>
        new(
            ns + "url",
            new XElement(ns + "loc", loc),
            new XElement(ns + "changefreq", changefreq),
            new XElement(ns + "priority", priority)
        );
}
