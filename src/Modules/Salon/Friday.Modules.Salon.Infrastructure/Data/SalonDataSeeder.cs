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
    public static IReadOnlyDictionary<string, string> ServiceImageUrls { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Cắt tóc"] = "/resources/dich_vu/cat_toc.jpg",
            ["Uốn / Duỗi"] = "/resources/dich_vu/uon_duoi.jpg",
            ["Nhuộm / Tẩy"] = "/resources/dich_vu/nhuom_tay.jpg",
            ["Nhuộm thiết kế"] = "/resources/dich_vu/nhuom_thiet_ke.jpg",
            ["Nối tóc"] = "/resources/dich_vu/noi_toc.jpg",
            ["Phục hồi / Olaplex"] = "/resources/dich_vu/phuc_hoi.jpg",
            ["Gội / Tạo kiểu"] = "/resources/dich_vu/goi_tao_kieu.jpg",
        };

    public static IReadOnlyList<PartnerDefinition> PartnerDefinitions { get; } =
        [
            new(
                "OLAPLEX",
                "/resources/doi_tac/olaplex.png",
                "Olaplex là một trong những thương hiệu chăm sóc tóc lớn nhất trên thế giới với hơn 100 bằng sáng chế.",
                1
            ),
            new(
                "MOROCCANOIL",
                "/resources/doi_tac/moroccanoil.png",
                "Moroccanoil là một thương hiệu chăm sóc tóc nổi tiếng toàn cầu và được các chuyên gia tạo mẫu tóc khuyên dùng.",
                2
            ),
            new(
                "B3 BRAZILIAN",
                "/resources/doi_tac/b3-brazilian.png",
                "B3 Brazillian Bond Builder thương hiệu nổi tiếng hàng đầu tại Mỹ, lựa chọn của nhiều salon chuyên nghiệp, giúp họ giải quyết mọi vấn đề hư tổn cao nhất của mái tóc và làm hài lòng cả những tín đồ yêu màu nhuộm khó tính.",
                3
            ),
            new(
                "L'Oréal",
                "/resources/doi_tac/loreal.png",
                "L'Oréal Paris là thương hiệu mỹ phẩm hàng đầu thế giới, giúp mọi người có thể tiếp cận những vẻ đẹp sang trọng nhất.",
                4
            ),
        ];

    public static IReadOnlyList<StylistDefinition> StylistDefinitions { get; } =
        [
            new("Lê Đình Ken", "/resources/stylist/le_dinh_ken.jpg", 1),
            new("Ngô Sỹ Minh", "/resources/stylist/ngo_sy_minh.jpg", 2),
            new("Nguyễn Doãn Chiến", "/resources/stylist/nguyen_doan_chien.jpg", 3),
        ];

    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default
    )
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
        await SeedPartnersAsync(repository, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedSettingsAsync(ISalonRepository repository, CancellationToken ct)
    {
        (string key, string value)[] settings =
        [
            ("site_name", "MC Hair Salon"),
            ("tagline", "Salon làm tóc hiện đại tại TP.HCM"),
            ("hotline", "0988305371"),
            ("email", "hello@mchair.vn"),
            ("address", "14D Cống Quỳnh, Phường Cầu Ông Lãnh, TP. Hồ Chí Minh, Việt Nam"),
            ("address_short", "14D Cống Quỳnh, P. Cầu Ông Lãnh, TP.HCM"),
            (
                "maps_url",
                "https://www.google.com/maps/search/?api=1&query=14D+C%E1%BB%91ng+Qu%E1%BB%B3nh,+Ph%C6%B0%E1%BB%9Dng+C%E1%BA%A7u+%C3%94ng+L%C3%A3nh,+TP.+H%E1%BB%93+Ch%C3%AD+Minh"
            ),
            ("opening_hours", "08:30 – 20:00 (Thứ 2 – Chủ nhật)"),
            ("facebook", "https://www.facebook.com/profile.php?id=61551835762411"),
            ("zalo", "0988305371"),
            ("messenger_url", "https://m.me/61551835762411"),
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
                Title = "MC Hair Salon",
                Subtitle = "Tuyên ngôn cá tính qua mái tóc",
                Body =
                    "Trải nghiệm làm đẹp hiện đại, tinh tế và cá nhân hoá — nơi bạn tìm thấy sự tự tin và phong cách riêng.",
                SortOrder = 1,
            },
            new()
            {
                SectionKey = "about",
                Title = "Về MC Hair",
                Body =
                    "MC Hair ra đời với sứ mệnh không chỉ tạo nên những kiểu tóc đẹp, mà còn đánh thức sự tự tin và thần thái riêng trong mỗi khách hàng.",
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
            new()
            {
                SectionKey = "partners_intro",
                Title = "Đối tác",
                Body =
                    "Kết hợp với các đối tác lớn, uy tín, bao gồm các nhãn sản phẩm chất lượng được sử dụng trong quy trình các dịch vụ đang vận hành tại hệ thống salon.",
                SortOrder = 10,
                IsVisible = true,
            },
            new()
            {
                SectionKey = "feedback_intro",
                Title = "Feedback khách hàng",
                Body =
                    "Dưới đây là những chia sẻ và cảm nhận của khách hàng khi sử dụng dịch vụ tại MC Hair Salon.",
                SortOrder = 8,
                IsVisible = true,
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
                Description = "Master / Hair artist — tư vấn kiểu phù hợp khuôn mặt",
                PriceFrom = 250_000,
                ImageUrl = ServiceImageUrls["Cắt tóc"],
                SortOrder = 1,
            },
            new()
            {
                Name = "Uốn / Duỗi",
                Description = "Uốn, duỗi, thuần chay — báo giá theo size tóc",
                PriceFrom = 1_000_000,
                ImageUrl = ServiceImageUrls["Uốn / Duỗi"],
                SortOrder = 2,
            },
            new()
            {
                Name = "Nhuộm / Tẩy",
                Description = "Nhuộm, tẩy, nâng sáng — sản phẩm chuyên nghiệp",
                PriceFrom = 800_000,
                ImageUrl = ServiceImageUrls["Nhuộm / Tẩy"],
                SortOrder = 3,
            },
            new()
            {
                Name = "Nhuộm thiết kế",
                Description = "Balayage, Ombre, Highlight, Hidden",
                PriceFrom = 1_000_000,
                ImageUrl = ServiceImageUrls["Nhuộm thiết kế"],
                SortOrder = 4,
            },
            new()
            {
                Name = "Nối tóc",
                Description = "Nối tóc tự nhiên — báo giá theo sợi / bó",
                PriceFrom = 25_000,
                ImageUrl = ServiceImageUrls["Nối tóc"],
                SortOrder = 5,
            },
            new()
            {
                Name = "Phục hồi / Olaplex",
                Description = "Olaplex, ATS, Keratin, Kerathphy",
                PriceFrom = 600_000,
                ImageUrl = ServiceImageUrls["Phục hồi / Olaplex"],
                SortOrder = 6,
            },
            new()
            {
                Name = "Gội / Tạo kiểu",
                Description = "Gội đầu, gội tóc nối, tạo kiểu",
                PriceFrom = 100_000,
                ImageUrl = ServiceImageUrls["Gội / Tạo kiểu"],
                SortOrder = 7,
            },
        ];

        foreach (HairService service in services)
        {
            await repository.AddServiceAsync(service, ct);
        }
    }

    private static async Task SeedStylistsAsync(ISalonRepository repository, CancellationToken ct)
    {
        foreach (StylistDefinition definition in StylistDefinitions)
        {
            await repository.AddStylistAsync(CreateStylist(definition), ct);
        }
    }

    public static async Task ApplyStylistDataAsync(
        ISalonRepository repository,
        CancellationToken ct = default
    )
    {
        IReadOnlyList<Stylist> existing = await repository.GetAllStylistsAsync(ct);

        foreach (StylistDefinition definition in StylistDefinitions)
        {
            Stylist stylist =
                existing.FirstOrDefault(x => x.SortOrder == definition.SortOrder)
                ?? existing.FirstOrDefault(x =>
                    string.Equals(x.Name, definition.Name, StringComparison.OrdinalIgnoreCase)
                )
                ?? new Stylist { SortOrder = definition.SortOrder, IsActive = true };

            stylist.Name = definition.Name;
            stylist.ImageUrl = definition.ImageUrl;
            stylist.Title = null;
            stylist.Bio = null;
            stylist.SortOrder = definition.SortOrder;
            stylist.IsActive = true;

            await repository.AddStylistAsync(stylist, ct);
        }
    }

    private static Stylist CreateStylist(StylistDefinition definition) =>
        new()
        {
            Name = definition.Name,
            ImageUrl = definition.ImageUrl,
            SortOrder = definition.SortOrder,
            IsActive = true,
        };

    private static Task SeedGalleryAsync(ISalonRepository repository, CancellationToken ct) =>
        Task.CompletedTask;

    private static async Task SeedPromotionsAsync(ISalonRepository repository, CancellationToken ct)
    {
        await repository.AddPromotionAsync(
            new Promotion
            {
                Title = "Giảm 20% dịch vụ nhuộm tháng này",
                Summary = "Ưu đãi dành cho khách hàng đặt lịch online.",
                Content =
                    "Áp dụng khi đặt lịch qua website MCHair. Không áp dụng cùng voucher khác.",
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

    private static async Task SeedTestimonialsAsync(
        ISalonRepository repository,
        CancellationToken ct
    )
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

    private static async Task SeedBeforeAfterAsync(
        ISalonRepository repository,
        CancellationToken ct
    )
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

    public static async Task SeedPartnersAsync(
        ISalonRepository repository,
        CancellationToken ct = default
    )
    {
        foreach (PartnerDefinition definition in PartnerDefinitions)
        {
            await repository.AddPartnerAsync(CreatePartner(definition), ct);
        }
    }

    public static async Task ApplyPartnerDataAsync(
        ISalonRepository repository,
        CancellationToken ct = default
    )
    {
        IReadOnlyList<Partner> existing = await repository.GetAllPartnersAsync(ct);

        foreach (PartnerDefinition definition in PartnerDefinitions)
        {
            Partner partner =
                existing.FirstOrDefault(x => x.SortOrder == definition.SortOrder)
                ?? existing.FirstOrDefault(x =>
                    string.Equals(x.Name, definition.Name, StringComparison.OrdinalIgnoreCase)
                )
                ?? new Partner { SortOrder = definition.SortOrder, IsActive = true };

            partner.Name = definition.Name;
            partner.Description = definition.Description;
            partner.LogoUrl = definition.LogoUrl;
            partner.WebsiteUrl = null;
            partner.SortOrder = definition.SortOrder;
            partner.IsActive = true;

            await repository.AddPartnerAsync(partner, ct);
        }
    }

    private static Partner CreatePartner(PartnerDefinition definition) =>
        new()
        {
            Name = definition.Name,
            Description = definition.Description,
            LogoUrl = definition.LogoUrl,
            SortOrder = definition.SortOrder,
            IsActive = true,
        };

    public static async Task ApplyServiceImagesAsync(
        ISalonRepository repository,
        CancellationToken ct = default
    )
    {
        IReadOnlyList<HairService> services = await repository.GetAllServicesAsync(ct);

        foreach (HairService service in services)
        {
            if (!ServiceImageUrls.TryGetValue(service.Name, out string? imageUrl))
            {
                continue;
            }

            if (string.Equals(service.ImageUrl, imageUrl, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            service.ImageUrl = imageUrl;
            await repository.AddServiceAsync(service, ct);
        }
    }
}

public sealed record PartnerDefinition(
    string Name,
    string LogoUrl,
    string Description,
    int SortOrder
);

public sealed record StylistDefinition(string Name, string ImageUrl, int SortOrder);
