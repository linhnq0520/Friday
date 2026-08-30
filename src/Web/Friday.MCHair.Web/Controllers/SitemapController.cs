using System.Text;
using System.Xml.Linq;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class SitemapController(ISalonRepository repository) : Controller
{
    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        string baseUrl = $"{Request.Scheme}://{Request.Host}";
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        List<XElement> urls =
        [
            CreateUrl(ns, baseUrl + "/", "daily", "1.0"),
            CreateUrl(ns, baseUrl + "/About", "monthly", "0.9"),
            CreateUrl(ns, baseUrl + "/Services", "weekly", "0.9"),
            CreateUrl(ns, baseUrl + "/Gallery", "weekly", "0.9"),
            CreateUrl(ns, baseUrl + "/khuyen-mai", "weekly", "0.9"),
            CreateUrl(ns, baseUrl + "/tin-tuc", "daily", "0.9"),
            CreateUrl(ns, baseUrl + "/Warranty", "monthly", "0.7"),
            CreateUrl(ns, baseUrl + "/Booking", "monthly", "0.9")
        ];

        try
        {
            IReadOnlyList<BlogPost> posts = await repository.GetPublishedBlogPostsAsync(
                null,
                1,
                100,
                null,
                cancellationToken
            );

            foreach (BlogPost post in posts)
            {
                urls.Add(CreateUrl(ns, $"{baseUrl}/tin-tuc/{post.Slug}", "weekly", "0.8"));
            }
        }
        catch
        {
            // Fallback gracefully if database query fails during sitemap generation
        }

        XElement urlset = new(ns + "urlset", urls);
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
