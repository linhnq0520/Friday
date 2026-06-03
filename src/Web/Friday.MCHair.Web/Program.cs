using Friday.BuildingBlocks.Application;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.BuildingBlocks.Application.Abstractions;
using Friday.MCHair.Web.Models;
using Friday.MCHair.Web.Services;
using Friday.Modules.Salon.Domain.Repositories;
using Friday.Modules.Salon.Application;
using Friday.Modules.Salon.Infrastructure;
using Friday.Modules.Salon.Infrastructure.Data;
using Friday.Modules.Salon.Infrastructure.Persistence;
using Friday.MCHair.Web.Cqrs;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Authentication.Cookies;
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
Directory.CreateDirectory(Path.GetDirectoryName(dbFile)!);
builder.Configuration["ConnectionStrings:FridayDb"] = $"Data Source={dbFile}";

builder.Services.AddBuildingBlocksApplication();
builder.Services.AddSalonApplication();
builder.Services.AddSalonInfrastructure(builder.Configuration);
builder.Services.AddLinKitCqrs();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IPriceListStore, PriceListStore>();
builder.Services.AddControllersWithViews();

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

await SalonDbMigrationStartup.ApplyMigrationsAsync(app.Services, app.Configuration);
await SalonDataSeeder.SeedAsync(app.Services);
await EnsurePriceListSeededAsync(app.Services);
await EnsureSiteContactSettingsAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task EnsurePriceListSeededAsync(IServiceProvider services)
{
    await using AsyncServiceScope scope = services.CreateAsyncScope();
    ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
    IReadOnlyDictionary<string, string> settings = await repository.GetSettingsAsync();
    if (settings.ContainsKey(PriceListStore.SettingKey))
    {
        return;
    }

    IPriceListStore store = scope.ServiceProvider.GetRequiredService<IPriceListStore>();
    await store.SaveAsync(PriceListDefaults.Create());
    IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
    await unitOfWork.CommitAsync();
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
        || zalo == "0900123456"
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

    if (changed)
    {
        await unitOfWork.CommitAsync();
    }
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
