using Friday.Modules.Salon.Application.Security;
using Friday.Modules.Salon.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;
using Friday.Modules.Salon.Domain.Repositories;
using Friday.Modules.Salon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Salon.Infrastructure.Data;

public static class SalonDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
        SalonDbContext db = scope.ServiceProvider.GetRequiredService<SalonDbContext>();

        IAdminPasswordService passwordService =
            scope.ServiceProvider.GetRequiredService<IAdminPasswordService>();

        if (!await repository.AnyAdminUsersAsync(cancellationToken))
        {
            await repository.AddAdminUserAsync(
                new AdminUser
                {
                    Username = "admin",
                    DisplayName = "MCHair Admin",
                    PasswordHash = passwordService.HashPassword("MCHair@2026"),
                },
                cancellationToken
            );
        }

        if ((await repository.GetAllServicesAsync(cancellationToken)).Count > 0)
        {
            await db.SaveChangesAsync(cancellationToken);
            return;
        }

        await SeedSettingsAsync(repository, cancellationToken);
        await SeedSectionsAsync(repository, cancellationToken);
        await SeedServicesAsync(repository, cancellationToken);
        await SeedStylistsAsync(repository, cancellationToken);
        await SeedGalleryAsync(repository, cancellationToken);
        await SeedPromotionsAsync(repository, cancellationToken);
        await SeedTestimonialsAsync(repository, cancellationToken);
        await SeedBeforeAfterAsync(repository, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedSettingsAsync(ISalonRepository repository, CancellationToken ct)
    {
        (string key, string value)[] settings =
        [
            ("site_name", "MCHair Salon"),
            ("tagline", "Salon làm tóc chuyên nghiệp"),
            ("hotline", "0900 123 456"),
            ("email", "hello@mchair.vn"),
            ("address", "123 Nguyễn Huệ, Quận 1, TP.HCM"),
            ("opening_hours", "9:00 - 20:00 (Thứ 2 - Chủ nhật)"),
            ("facebook", "https://facebook.com/mchair"),
            ("zalo", "0900123456"),
            ("instagram", "https://instagram.com/mchair"),
            ("seo_title", "MCHair - Salon cắt tóc, nhuộm, uốn tại TP.HCM"),
            (
                "seo_description",
                "MCHair Salon chuyên cắt tóc, nhuộm, uốn, phục hồi hư tổn. Đặt lịch online, đội ngũ stylist giàu kinh nghiệm tại TP.HCM."
            ),
        ];

        foreach ((string key, string value) in settings)
        {
            await repository.UpsertSettingAsync(key, value, ct);
        }
    }

    private static async Task SeedSectionsAsync(ISalonRepository repository, CancellationToken ct)
    {
        SiteSection[] sections =
        [
            new()
            {
                SectionKey = "hero",
                Title = "MCHair Salon",
                Subtitle = "Nơi tôn vinh vẻ đẹp mái tóc của bạn",
                Body =
                    "Chào mừng bạn đến với MCHair — salon làm tóc hiện đại với dịch vụ cắt, nhuộm, uốn và phục hồi chuyên sâu.",
                SortOrder = 1,
            },
            new()
            {
                SectionKey = "about",
                Title = "Về MCHair",
                Body =
                    "MCHair tự hào mang đến trải nghiệm làm đẹp tinh tế với đội ngũ stylist tận tâm và sản phẩm chăm sóc tóc cao cấp.",
                SortOrder = 2,
            },
            new()
            {
                SectionKey = "gallery_intro",
                Title = "Mẫu tóc hot",
                Subtitle = "Bộ sưu tập xu hướng 2026",
                Body = "Khám phá những kiểu tóc và màu nhuộm thịnh hành giúp bạn tỏa sáng.",
                SortOrder = 3,
            },
            new()
            {
                SectionKey = "services_intro",
                Title = "Dịch vụ tóc",
                Subtitle = "Giá minh bạch — chất lượng đảm bảo",
                Body = "Các dịch vụ làm tóc phổ biến tại salon với mức giá cạnh tranh.",
                SortOrder = 4,
            },
        ];

        foreach (SiteSection section in sections)
        {
            await repository.AddSectionAsync(section, ct);
        }
    }

    private static async Task SeedServicesAsync(ISalonRepository repository, CancellationToken ct)
    {
        HairService[] services =
        [
            new()
            {
                Name = "Cắt tóc",
                Description = "Cắt & tạo kiểu theo khuôn mặt",
                PriceFrom = 200_000,
                SortOrder = 1,
            },
            new()
            {
                Name = "Uốn / Duỗi / Nhuộm",
                Description = "Hóa chất an toàn, màu bền đẹp",
                PriceFrom = 350_000,
                SortOrder = 2,
            },
            new()
            {
                Name = "Phục hồi",
                Description = "Hấp collagen, phục hồi hư tổn",
                PriceFrom = 300_000,
                SortOrder = 3,
            },
            new()
            {
                Name = "Nối tóc",
                Description = "Nối tóc tự nhiên, tăng volume",
                PriceFrom = 1_500_000,
                SortOrder = 4,
            },
        ];

        foreach (HairService service in services)
        {
            await repository.AddServiceAsync(service, ct);
        }
    }

    private static async Task SeedStylistsAsync(ISalonRepository repository, CancellationToken ct)
    {
        Stylist[] stylists =
        [
            new()
            {
                Name = "Minh Anh",
                Title = "Senior Stylist",
                Bio = "8 năm kinh nghiệm nhuộm & tạo kiểu.",
                SortOrder = 1,
            },
            new()
            {
                Name = "Hoàng Long",
                Title = "Hair Designer",
                Bio = "Chuyên cắt nam nữ và Hush Cut.",
                SortOrder = 2,
            },
            new()
            {
                Name = "Thu Hà",
                Title = "Color Expert",
                Bio = "Chuyên gia màu thời trang và balayage.",
                SortOrder = 3,
            },
        ];

        foreach (Stylist stylist in stylists)
        {
            await repository.AddStylistAsync(stylist, ct);
        }
    }

    private static async Task SeedGalleryAsync(ISalonRepository repository, CancellationToken ct)
    {
        GalleryItem[] items =
        [
            new()
            {
                Title = "Màu thời trang",
                Category = GalleryCategory.FashionColor,
                ImageUrl = "/images/placeholders/color.jpg",
                SortOrder = 1,
            },
            new()
            {
                Title = "Kiểu tóc thịnh hành",
                Category = GalleryCategory.TrendingStyle,
                ImageUrl = "/images/placeholders/style.jpg",
                SortOrder = 2,
            },
            new()
            {
                Title = "Phục hồi hư tổn",
                Category = GalleryCategory.HairRecovery,
                ImageUrl = "/images/placeholders/recovery.jpg",
                SortOrder = 3,
            },
            new()
            {
                Title = "Nối tóc",
                Category = GalleryCategory.HairExtensions,
                ImageUrl = "/images/placeholders/extensions.jpg",
                SortOrder = 4,
            },
        ];

        foreach (GalleryItem item in items)
        {
            await repository.AddGalleryItemAsync(item, ct);
        }
    }

    private static async Task SeedPromotionsAsync(ISalonRepository repository, CancellationToken ct)
    {
        await repository.AddPromotionAsync(
            new Promotion
            {
                Title = "Giảm 20% dịch vụ nhuộm tháng này",
                Summary = "Ưu đãi dành cho khách hàng đặt lịch online.",
                Content = "Áp dụng khi đặt lịch qua website MCHair. Không áp dụng cùng voucher khác.",
                PublishedAt = DateTime.UtcNow.AddDays(-3),
            },
            ct
        );

        await repository.AddPromotionAsync(
            new Promotion
            {
                Title = "Tặng gói hấp dưỡng khi cắt + nhuộm",
                Summary = "Combo làm đẹp tiết kiệm cho mùa lễ.",
                Content = "Hấp collagen miễn phí khi sử dụng combo cắt tóc và nhuộm full.",
                PublishedAt = DateTime.UtcNow.AddDays(-10),
            },
            ct
        );
    }

    private static async Task SeedTestimonialsAsync(ISalonRepository repository, CancellationToken ct)
    {
        Testimonial[] items =
        [
            new()
            {
                CustomerName = "Lan Nguyễn",
                Content = "Màu nhuộm đẹp, stylist tư vấn rất kỹ. Sẽ quay lại!",
                Rating = 5,
                SortOrder = 1,
            },
            new()
            {
                CustomerName = "Phạm Tuấn",
                Content = "Cắt Hush Cut chuẩn trend, không gian salon sạch sẽ.",
                Rating = 5,
                SortOrder = 2,
            },
            new()
            {
                CustomerName = "Mai Trang",
                Content = "Phục hồi tóc hư tổn hiệu quả sau 2 lần hấp.",
                Rating = 5,
                SortOrder = 3,
            },
        ];

        foreach (Testimonial item in items)
        {
            await repository.AddTestimonialAsync(item, ct);
        }
    }

    private static async Task SeedBeforeAfterAsync(ISalonRepository repository, CancellationToken ct)
    {
        BeforeAfterItem[] items =
        [
            new()
            {
                Title = "Hush Cut",
                BeforeImageUrl = "/images/placeholders/before1.jpg",
                AfterImageUrl = "/images/placeholders/after1.jpg",
                SortOrder = 1,
            },
            new()
            {
                Title = "Nhuộm + uốn xoăn lơi",
                BeforeImageUrl = "/images/placeholders/before2.jpg",
                AfterImageUrl = "/images/placeholders/after2.jpg",
                SortOrder = 2,
            },
        ];

        foreach (BeforeAfterItem item in items)
        {
            await repository.AddBeforeAfterAsync(item, ct);
        }
    }
}
