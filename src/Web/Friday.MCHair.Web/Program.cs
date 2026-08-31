using System.Globalization;
using Friday.BuildingBlocks.Application;
using Friday.BuildingBlocks.Application.Abstractions;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.MCHair.Web.Cqrs;
using Friday.MCHair.Web.Localization;
using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Application;
using Friday.Modules.Salon.Application.Models;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using Friday.Modules.Salon.Infrastructure;
using Friday.Modules.Salon.Infrastructure.Data;
using Friday.Modules.Salon.Infrastructure.Persistence;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

string contentRoot = AppContext.BaseDirectory;

WebApplicationBuilder builder = WebApplication.CreateBuilder(
    new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = contentRoot,
        WebRootPath = Path.Combine(contentRoot, "wwwroot"),
    }
);

string dbFile = Path.Combine(contentRoot, "Data", "mchair.db");
string projectDb = Path.Combine(Directory.GetCurrentDirectory(), "Data", "mchair.db");
if (File.Exists(projectDb) && (!File.Exists(dbFile) || (new FileInfo(projectDb).Length > new FileInfo(dbFile).Length && new FileInfo(dbFile).Length < 10000)))
{
    Directory.CreateDirectory(Path.GetDirectoryName(dbFile)!);
    try
    {
        File.Copy(projectDb, dbFile, true);
    }
    catch { /* Ignore */ }
}

Directory.CreateDirectory(Path.GetDirectoryName(dbFile)!);
builder.Configuration["ConnectionStrings:FridayDb"] = $"Data Source={dbFile}";

builder.Services.AddBuildingBlocksApplication();
builder.Services.AddSalonApplication();
builder.Services.AddSalonInfrastructure(builder.Configuration);
builder.Services.AddLinKitCqrs();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IPriceListStore, PriceListStore>();
builder.Services.AddScoped<IWarrantyStore, WarrantyStore>();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddSingleton<IUiLocalizer, UiLocalizer>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

WebApplication app = builder.Build();

RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(CultureConstants.Vietnamese)
    .AddSupportedCultures(CultureConstants.SupportedCultures)
    .AddSupportedUICultures(CultureConstants.SupportedCultures);

localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new CookieRequestCultureProvider(),
    new AcceptLanguageHeaderRequestCultureProvider(),
};

app.UseRequestLocalization(localizationOptions);

await SalonDbMigrationStartup.ApplyMigrationsAsync(app.Services, app.Configuration);
await SalonDataSeeder.SeedAsync(app.Services);
await EnsurePriceListSeededAsync(app.Services);
await EnsureWarrantySeededAsync(app.Services);
await EnsureEnglishContentSeededAsync(app.Services);
await EnsureSiteContactSettingsAsync(app.Services);
await EnsureGalleryFromResourcesAsync(app.Services);
await EnsureShowcaseFromResourcesAsync(app.Services);
await EnsurePartnersSeededAsync(app.Services);
await EnsureServiceImagesAsync(app.Services);
await EnsureStylistsFromResourcesAsync(app.Services);
await SalonDataSeeder.EnsureBlogPostsSeededAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(
    new StaticFileOptions
    {
        OnPrepareResponse = static ctx =>
        {
            string path = ctx.Context.Request.Path.Value ?? string.Empty;
            if (
                path.Contains("favicon", StringComparison.OrdinalIgnoreCase)
                || path.Contains("apple-touch-icon", StringComparison.OrdinalIgnoreCase)
            )
            {
                IHeaderDictionary headers = ctx.Context.Response.Headers;
                headers.CacheControl = "no-cache, no-store, must-revalidate";
                headers.Pragma = "no-cache";
                headers.Expires = "0";
            }
        },
    }
);
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task EnsurePriceListSeededAsync(IServiceProvider services)
{
    const string priceListVersionKey = "price_list_version";
    const string currentPriceListVersion = "2026-06-menu";

    await using AsyncServiceScope scope = services.CreateAsyncScope();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IPriceListStore store = scope.ServiceProvider.GetRequiredService<IPriceListStore>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync();

    bool needsSeed = !settings.ContainsKey(PriceListStore.SettingKey);
    bool needsVersionSync =
        settings.GetValueOrDefault(priceListVersionKey) != currentPriceListVersion;

    if (!needsSeed && !needsVersionSync)
    {
        return;
    }

    await store.SaveAsync(PriceListDefaults.Create());
    await repository.UpsertSettingAsync(priceListVersionKey, currentPriceListVersion);
    await unitOfWork.CommitAsync();
}

static async Task EnsureWarrantySeededAsync(IServiceProvider services)
{
    const string warrantyVersionKey = "warranty_page_version";
    const string currentWarrantyVersion = "2026-06-periods";

    await using AsyncServiceScope scope = services.CreateAsyncScope();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IWarrantyStore store = scope.ServiceProvider.GetRequiredService<IWarrantyStore>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync();

    bool needsSeed = !settings.ContainsKey(WarrantyStore.SettingKey);
    bool needsPeriodSync =
        settings.GetValueOrDefault(warrantyVersionKey) != currentWarrantyVersion;

    if (!needsSeed && !needsPeriodSync)
    {
        return;
    }

    if (needsSeed)
    {
        await store.SaveAsync(WarrantyDefaults.Create());
    }
    else
    {
        WarrantyPageData data = await store.GetAsync();
        WarrantySectionData? periodsSection = data.Sections.FirstOrDefault(section =>
            section.Title.Contains("Thời gian bảo hành", StringComparison.OrdinalIgnoreCase)
        );

        if (periodsSection is not null)
        {
            periodsSection.Body = WarrantyDefaults.WarrantyPeriodsBody;
            await store.SaveAsync(data);
        }
    }

    await repository.UpsertSettingAsync(warrantyVersionKey, currentWarrantyVersion);
    await unitOfWork.CommitAsync();
}

static async Task EnsureEnglishContentSeededAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync();
    bool changed = false;

    if (!settings.ContainsKey(PriceListStore.SettingKeyEn))
    {
        string json = System.Text.Json.JsonSerializer.Serialize(
            PriceListDefaultsEn.Create(),
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            }
        );
        await repository.UpsertSettingAsync(PriceListStore.SettingKeyEn, json);
        changed = true;
    }

    if (!settings.ContainsKey(WarrantyStore.SettingKeyEn))
    {
        string json = System.Text.Json.JsonSerializer.Serialize(
            WarrantyDefaultsEn.Create(),
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            }
        );
        await repository.UpsertSettingAsync(WarrantyStore.SettingKeyEn, json);
        changed = true;
    }

    if (changed)
    {
        await unitOfWork.CommitAsync();
    }
}

static async Task EnsureSiteContactSettingsAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    bool changed = false;

    if (
        !settings.TryGetValue("address", out string? address)
        || string.IsNullOrWhiteSpace(address)
        || address.Contains("Nguyễn Huệ", StringComparison.OrdinalIgnoreCase)
    )
    {
        await repository.UpsertSettingAsync("address", SiteContent.DefaultAddress);
        changed = true;
    }

    if (!settings.ContainsKey("address_short"))
    {
        await repository.UpsertSettingAsync("address_short", SiteContent.DefaultAddressShort);
        changed = true;
    }

    if (!settings.ContainsKey("maps_url"))
    {
        await repository.UpsertSettingAsync("maps_url", SiteContent.DefaultMapsUrl);
        changed = true;
    }

    if (
        !settings.TryGetValue("zalo", out string? zalo)
        || string.IsNullOrWhiteSpace(zalo)
        || zalo == SiteContent.DefaultZaloPhone
    )
    {
        await repository.UpsertSettingAsync("zalo", SiteContent.DefaultZaloPhone);
        changed = true;
    }

    if (!settings.ContainsKey("messenger_url"))
    {
        await repository.UpsertSettingAsync("messenger_url", SiteContent.DefaultMessengerUrl);
        changed = true;
    }

    if (!settings.ContainsKey(BookingSettings.ModeKey))
    {
        await repository.UpsertSettingAsync(
            BookingSettings.ModeKey,
            SiteContent.DefaultBookingMode
        );
        changed = true;
    }

    if (!settings.ContainsKey(BookingSettings.ExternalUrlKey))
    {
        await repository.UpsertSettingAsync(
            BookingSettings.ExternalUrlKey,
            SiteContent.DefaultBookingEasySalonUrl
        );
        changed = true;
    }

    if (changed)
    {
        await unitOfWork.CommitAsync();
    }
}

static async Task EnsureGalleryFromResourcesAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    IWebHostEnvironment environment =
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    string root = Path.Combine(environment.WebRootPath, "resources", "bo_suu_tap");
    if (!Directory.Exists(root))
    {
        return;
    }

    IReadOnlyList<GalleryItem> existing = await repository.GetAllGalleryAsync();
    HashSet<string> existingUrls = existing
        .Select(x => x.ImageUrl.Trim().Replace('\\', '/'))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    bool hasPlaceholders = existing.Any(x =>
        x.ImageUrl.Contains("/images/placeholders/", StringComparison.OrdinalIgnoreCase)
    );

    int sortOrder = existing.Count == 0 ? 0 : existing.Max(x => x.SortOrder);
    bool changed = false;

    foreach (GalleryItem item in existing)
    {
        if (!item.ImageUrl.StartsWith("/resources/bo_suu_tap/", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        string physicalPath = Path.Combine(
            environment.WebRootPath,
            item.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
        );

        if (!File.Exists(physicalPath))
        {
            await repository.DeleteGalleryItemAsync(item);
            existingUrls.Remove(item.ImageUrl.Trim().Replace('\\', '/'));
            changed = true;
        }
    }

    foreach (GalleryCategory category in GalleryCategoryInfo.CollectionCategories)
    {
        string folder = GalleryCategoryInfo.GetFolderSlug(category);
        string folderPath = Path.Combine(root, folder);
        if (!Directory.Exists(folderPath))
        {
            continue;
        }

        foreach (
            string file in Directory
                .EnumerateFiles(folderPath, "*", SearchOption.AllDirectories)
                .Where(IsGalleryImageFile)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
        )
        {
            string relative =
                "/resources/bo_suu_tap/"
                + folder
                + "/"
                + Path.GetRelativePath(folderPath, file).Replace('\\', '/');

            if (existingUrls.Contains(relative))
            {
                continue;
            }

            sortOrder++;
            await repository.AddGalleryItemAsync(
                new GalleryItem
                {
                    Title = Path.GetFileNameWithoutExtension(file),
                    Category = category,
                    ImageUrl = relative,
                    SortOrder = sortOrder,
                    IsPublished = true,
                }
            );
            existingUrls.Add(relative);
            changed = true;
        }
    }

    if (hasPlaceholders && changed)
    {
        foreach (
            GalleryItem placeholder in existing
                .Where(x =>
                    x.ImageUrl.Contains("/images/placeholders/", StringComparison.OrdinalIgnoreCase)
                )
                .ToList()
        )
        {
            await repository.DeleteGalleryItemAsync(placeholder);
            changed = true;
        }
    }

    if (changed)
    {
        await unitOfWork.CommitAsync();
    }
}

static async Task EnsureShowcaseFromResourcesAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    IWebHostEnvironment environment =
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    IReadOnlyList<ShowcaseItem> existing = await repository.GetAllShowcaseAsync(null);
    HashSet<string> existingUrls = existing
        .Select(x => x.ImageUrl.Trim().Replace('\\', '/'))
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    int sortOrder = existing.Count == 0 ? 0 : existing.Max(x => x.SortOrder);
    bool changed = false;

    foreach (ShowcaseType type in ShowcaseTypeInfo.AllTypes)
    {
        string folder = ShowcaseTypeInfo.GetFolderSlug(type);
        string folderPath = Path.Combine(environment.WebRootPath, "resources", folder);
        if (!Directory.Exists(folderPath))
        {
            continue;
        }

        foreach (
            string file in Directory
                .EnumerateFiles(folderPath, "*", SearchOption.AllDirectories)
                .Where(IsGalleryImageFile)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
        )
        {
            string relative =
                $"/resources/{folder}/" + Path.GetRelativePath(folderPath, file).Replace('\\', '/');

            if (existingUrls.Contains(relative))
            {
                continue;
            }

            sortOrder++;
            await repository.AddShowcaseItemAsync(
                new ShowcaseItem
                {
                    Title = Path.GetFileNameWithoutExtension(file),
                    Type = type,
                    ImageUrl = relative,
                    SortOrder = sortOrder,
                    IsPublished = true,
                }
            );
            existingUrls.Add(relative);
            changed = true;
        }
    }

    if (changed)
    {
        await unitOfWork.CommitAsync();
    }
}

static bool IsGalleryImageFile(string path)
{
    string extension = Path.GetExtension(path);
    return extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".webp", StringComparison.OrdinalIgnoreCase)
        || extension.Equals(".gif", StringComparison.OrdinalIgnoreCase);
}

static async Task EnsurePartnersSeededAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    bool changed = false;

    if ((await repository.GetAllPartnersAsync()).Count == 0)
    {
        await SalonDataSeeder.SeedPartnersAsync(repository);
        changed = true;
    }

    bool hasPartnerLogos = SalonDataSeeder.PartnerDefinitions.Any(definition =>
    {
        string physicalPath = Path.Combine(
            scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>().WebRootPath,
            definition.LogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
        );
        return File.Exists(physicalPath);
    });

    if (hasPartnerLogos)
    {
        await SalonDataSeeder.ApplyPartnerDataAsync(repository);
        changed = true;
    }

    SiteSection? existingPartnersIntro = await repository.GetSectionByKeyAsync("partners_intro");

    if (hasPartnerLogos || existingPartnersIntro is null)
    {
        await repository.AddSectionAsync(
            new SiteSection
            {
                Id = existingPartnersIntro?.Id ?? 0,
                SectionKey = "partners_intro",
                Title = "Đối tác",
                Body =
                    "Kết hợp với các đối tác lớn, uy tín, bao gồm các nhãn sản phẩm chất lượng được sử dụng trong quy trình các dịch vụ đang vận hành tại hệ thống salon.",
                SortOrder = 10,
                IsVisible = true,
            }
        );
        changed = true;
    }

    if (await repository.GetSectionByKeyAsync("feedback_intro") is null)
    {
        await repository.AddSectionAsync(
            new SiteSection
            {
                SectionKey = "feedback_intro",
                Title = "Feedback khách hàng",
                Body =
                    "Dưới đây là những chia sẻ và cảm nhận của khách hàng khi sử dụng dịch vụ tại MC Hair Salon.",
                SortOrder = 8,
                IsVisible = true,
            }
        );
        changed = true;
    }

    if (changed)
    {
        await unitOfWork.CommitAsync();
    }
}

static async Task EnsureStylistsFromResourcesAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    IWebHostEnvironment environment =
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    bool hasStylistImages = SalonDataSeeder.StylistDefinitions.Any(definition =>
    {
        string physicalPath = Path.Combine(
            environment.WebRootPath,
            definition.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
        );
        return File.Exists(physicalPath);
    });

    if (!hasStylistImages)
    {
        return;
    }

    await SalonDataSeeder.ApplyStylistDataAsync(repository);
    await unitOfWork.CommitAsync();
}

static async Task EnsureServiceImagesAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    IWebHostEnvironment environment =
        scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

    bool hasMappedFiles = SalonDataSeeder.ServiceImageUrls.Values.Any(imageUrl =>
    {
        string physicalPath = Path.Combine(
            environment.WebRootPath,
            imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)
        );
        return File.Exists(physicalPath);
    });

    if (!hasMappedFiles)
    {
        return;
    }

    await SalonDataSeeder.ApplyServiceImagesAsync(repository);
    await unitOfWork.CommitAsync();
}

namespace Friday.MCHair.Web
{
    public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SalonDbContext>
    {
        public SalonDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<SalonDbContext> options = new();
            string connectionString =
                Environment.GetEnvironmentVariable("MCHAIR_DESIGN_TIME_SQLITE")
                ?? "Data Source=mchair.dev.db";
            options.UseSqlite(connectionString);
            return new SalonDbContext(options.Options);
        }
    }
}
