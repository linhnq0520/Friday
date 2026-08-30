using Friday.BuildingBlocks.Application.Abstractions;
using Friday.MCHair.Web.Localization;
using Friday.Modules.Salon.Application.Features;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace Friday.MCHair.Web.Controllers;

public sealed class NewsController(
    IMediator mediator,
    ISalonRepository repository,
    IUnitOfWork unitOfWork,
    IUiLocalizer localizer
) : Controller
{
    [HttpGet("/tin-tuc")]
    [HttpGet("/bai-viet")]
    [HttpGet("/News")]
    public async Task<IActionResult> Index(
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] string? q = null,
        CancellationToken cancellationToken = default
    )
    {
        BlogListResultDto result = await mediator.QueryAsync(
            new GetBlogPostsPageQuery(category, page, 9, q),
            cancellationToken
        );

        string pageTitle = !string.IsNullOrWhiteSpace(category)
            ? $"{category} | {localizer["Meta_News"].Value}"
            : localizer["Meta_News"].Value;

        ViewData["Title"] = pageTitle;
        ViewData["MetaDescription"] = localizer["Meta_NewsDescription"].Value;
        return View(result);
    }

    [HttpGet("/tin-tuc/{slug}")]
    [HttpGet("/bai-viet/{slug}")]
    [HttpGet("/News/{slug}")]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        BlogPostDetailDto? post = await mediator.QueryAsync(
            new GetBlogPostDetailQuery(slug),
            cancellationToken
        );

        if (post is null)
        {
            return NotFound();
        }

        try
        {
            await repository.IncrementBlogPostViewAsync(post.Id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            // Do not fail page rendering if view counter fails
        }

        ViewData["Title"] = post.MetaTitle ?? $"{post.Title} | MC Hair Salon";
        ViewData["MetaDescription"] = post.MetaDescription ?? post.Summary ?? post.Title;
        ViewData["MetaKeywords"] = post.MetaKeywords;
        ViewData["OgImage"] = post.ThumbnailUrl;

        return View(post);
    }
}
