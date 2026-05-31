using Friday.BuildingBlocks.Application;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.MCHair.Web.Services;
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
