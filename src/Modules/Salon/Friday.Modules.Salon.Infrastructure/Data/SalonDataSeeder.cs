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

        await EnsureSchemaUpdatesAsync(db, cancellationToken);

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
                    Role = AdminRole.Admin,
                    IsActive = true,
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
        await SeedBlogPostsAsync(repository, cancellationToken);
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
                Title = "GIẢM 50% DỊCH VỤ HÓA CHẤT",
                Summary = "Giảm 50% dịch vụ hóa chất khi khách đặt lịch làm đẹp vào khung giờ vàng từ 9:00am đến 12:00pm",
                Content = "💥 **KHUNG GIỜ VÀNG – ƯU ĐÃI CỰC SỐC TẠI MC HAIR** 💥\n⏰ **09:00 – 12:00** 🔥 **GIẢM NGAY 50%** tất cả **dịch vụ hóa chất** (Uốn – Nhuộm – Duỗi).\n⏰ **Sau 12:00** ❤️ Vẫn nhận ngay **GIẢM 30%** tất cả dịch vụ hóa chất.\n📌 Cơ hội làm đẹp với mức giá ưu đãi nhất – Đừng bỏ lỡ!\n📍 MC Hair Salon 🏠 14D Cống Quỳnh, Phường Cầu Ông Lãnh, TP. Hồ Chí Minh.\n📥 Inbox ngay để giữ chỗ trong **khung giờ vàng** và nhận ưu đãi hấp dẫn!\n#MCHair #KhungGioVang #Giam50 #Giam30 #UonNhuomDuoi #SalonTPHCM",
                ImageUrl = "/resources/dich_vu/nhuom_thiet_ke.jpg",
                PublishedAt = DateTime.UtcNow.AddDays(-3),
            },
            ct
        );

        await repository.AddPromotionAsync(
            new Promotion
            {
                Title = "Tặng Cắt Và Hấp Phục Hồi",
                Summary = "Tặng cắt và hấp phục hồi trải nghiệm dịch vụ hoá chất tại MC Hair",
                Content = "✨ **Làm đẹp xứng đáng với những đặc quyền tốt nhất tại MC Hair** ✨\nMột mái tóc đẹp không chỉ nằm ở màu nhuộm hay kiểu uốn, mà còn ở chất tóc khỏe và một đường cắt chuẩn giúp tôn lên đường nét gương mặt.\nKhi trải nghiệm bất kỳ dịch vụ hóa chất (Uốn/Nhuộm/Duỗi) tại MC Hair, bạn sẽ được tặng kèm ngay gói Cắt thiết kế form dáng và Hấp dưỡng phục hồi tóc đa tầng.",
                ImageUrl = "/resources/dich_vu/phuc_hoi.jpg",
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

    public static async Task EnsureBlogPostsSeededAsync(
        IServiceProvider services,
        CancellationToken ct = default
    )
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
        SalonDbContext db = scope.ServiceProvider.GetRequiredService<SalonDbContext>();

        IReadOnlyList<BlogPost> existing = await repository.GetAllBlogPostsAsync(null, null, ct);
        if (existing.Count > 0)
        {
            return;
        }

        await SeedBlogPostsAsync(repository, ct);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedBlogPostsAsync(ISalonRepository repository, CancellationToken ct)
    {
        BlogPost[] posts =
        [
            new BlogPost
            {
                Title = "[Video] Trải Nghiệm Quy Trình Tạo Mẫu & Nhuộm Tóc Thiết Kế Tại MC Hair Salon",
                Slug = "video-trai-nghiem-quy-trinh-tao-mau-nhuom-toc-thiet-ke-mc-hair-salon",
                Category = "Xu hướng tóc",
                Summary = "Video cận cảnh quy trình tư vấn 1:1, kỹ thuật nhuộm chuyển sắc Balayage và chăm sóc phục hồi chuyên sâu tại kênh YouTube @mchairsalon.",
                ThumbnailUrl = "/resources/dich_vu/997d6f7b2107436a9338c7f6ec2547cb.jpeg",
                VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                AuthorName = "MC Hair Team",
                PublishedAt = DateTime.UtcNow,
                IsPublished = true,
                IsFeatured = true,
                ViewCount = 238,
                MetaTitle = "Trải Nghiệm Thực Tế Tại MC Hair Salon | Kênh YouTube @mchairsalon",
                MetaDescription = "Xem video thực tế quy trình làm tóc chuyên nghiệp tại MC Hair Salon và đăng ký theo dõi kênh YouTube @mchairsalon.",
                Content = """
                <p class="lead">Chào mừng bạn đến với kênh chính thức của <strong>MC Hair Salon</strong>! Trong video dưới đây, hãy cùng theo chân đội ngũ Stylist khám phá quy trình biến hình mái tóc từ khâu kiểm tra chất tóc, tư vấn phối màu cho đến bước tạo kiểu hoàn thiện đầy ấn tượng.</p>

                <h2>Khám Phá Kỹ Thuật Tạo Mẫu Độc Bản Tại MC Hair</h2>
                <p>Mỗi mái tóc tại MC Hair đều là một tác phẩm nghệ thuật cá nhân hóa, được thiết kế tỉ mỉ dựa trên cấu trúc gương mặt, phong cách và chất tóc riêng của từng khách hàng.</p>

                <div class="embedded-video-wrapper">
                    <iframe src="https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ" allowfullscreen loading="lazy" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"></iframe>
                </div>

                <h2>Những Điểm Nổi Bật Trong Quy Trình Của Chúng Tôi:</h2>
                <ul>
                    <li><strong>Tư vấn 1:1 chuyên sâu:</strong> Lắng nghe mong muốn và phân tích cấu trúc sợi tóc kỹ lưỡng trước khi bắt đầu.</li>
                    <li><strong>Sản phẩm cao cấp:</strong> Sử dụng 100% dòng mỹ phẩm tóc hàng đầu thế giới (Olaplex, L'Oréal Professionnel, Moroccanoil).</li>
                    <li><strong>Kỹ thuật điêu luyện:</strong> Đội ngũ Stylist nhiều năm kinh nghiệm, liên tục cập nhật các xu hướng tóc quốc tế hot nhất.</li>
                    <li><strong>Chế độ bảo hành tận tâm:</strong> Cam kết đồng hành và chăm sóc mái tóc khách hàng sau dịch vụ.</li>
                </ul>

                <blockquote>"Đừng quên nhấn Đăng ký (Subscribe) kênh YouTube chính thức của chúng tôi tại <a href='https://www.youtube.com/@mchairsalon' target='_blank' rel='noopener'>@mchairsalon</a> để cập nhật những video chia sẻ mẹo làm đẹp và các kiểu tóc mới nhất nhé!"</blockquote>
                """
            },
            new BlogPost
            {
                Title = "Top 5 Xu Hướng Màu Nhuộm Balayage & Highlight Đẹp Nhất 2026",
                Slug = "top-5-xu-huong-mau-nhuom-balayage-highlight-2026",
                Category = "Xu hướng tóc",
                Summary = "Khám phá các tông màu Balayage khói, nâu trà sữa và highlight thiết kế đang dẫn đầu xu hướng làm đẹp tại MC Hair Salon.",
                ThumbnailUrl = "/resources/dich_vu/997d6f7b2107436a9338c7f6ec2547cb.jpeg",
                AuthorName = "MC Hair Stylist",
                PublishedAt = DateTime.UtcNow.AddDays(-2),
                IsPublished = true,
                IsFeatured = true,
                ViewCount = 142,
                MetaTitle = "Top 5 Xu Hướng Màu Nhuộm Balayage & Highlight Đẹp Nhất 2026 | MC Hair",
                MetaDescription = "Tổng hợp những kiểu nhuộm Balayage và highlight thiết kế hot nhất năm 2026, tư vấn tông màu nhuộm tôn da chuẩn salon.",
                Content = """
                <p class="lead">Nhuộm Balayage và Highlight thiết kế tiếp tục giữ vị trí độc tôn trong làng tạo mẫu tóc năm 2026. Kỹ thuật pha phối màu chuyển sắc mềm mại không chỉ mang đến vẻ đẹp sang trọng mà còn giúp mái tóc trông dày dặn và có chiều sâu hơn.</p>

                <h2>1. Balayage Nâu Tây Ánh Khói (Smoky Ash Balayage)</h2>
                <p>Tông màu thời thượng kết hợp giữa nền nâu tây ấm áp và các vệt sáng ánh khói bạc tinh tế. Điểm đặc biệt của kiểu nhuộm này là khi tóc mọc dài ra từ chân, phần tóc đen nguyên bản sẽ hòa quyện tự nhiên, không lộ ranh giới màu.</p>

                <h2>2. Nâu Trà Sữa Caramel (Caramel Milk Tea)</h2>
                <p>Dành riêng cho những cô nàng yêu thích phong cách ngọt ngào nhưng không kém phần thanh lịch. Ánh caramel sáng nhẹ giúp làm sáng bừng làn da châu Á và cực kỳ dễ phối đồ.</p>

                <h2>3. Babylights & Money Piece Cá Tính</h2>
                <p>Kỹ thuật highlight sợi mảnh (Babylights) toàn đầu kết hợp cùng viền tóc sáng màu ôm trọn gương mặt (Money Piece) tạo điểm nhấn hút mắt tức thì trong mọi góc nhìn.</p>

                <blockquote>"Tại MC Hair Salon, mỗi mái tóc nhuộm thiết kế đều được tính toán theo tỉ lệ gương mặt, màu da và chất tóc tự nhiên của khách hàng để tạo nên tác phẩm độc bản."</blockquote>

                <h2>Quy Trình Nhuộm Chuẩn Chuyên Nghiệp Tại MC Hair</h2>
                <ul>
                    <li><strong>Bước 1:</strong> Khám và kiểm tra độ đàn hồi, cấu trúc sợi tóc.</li>
                    <li><strong>Bước 2:</strong> Tư vấn bảng màu và kỹ thuật phối sáng cá nhân hóa.</li>
                    <li><strong>Bước 3:</strong> Sử dụng thuốc tẩy/nhuộm kết hợp dưỡng phục hồi Olaplex chính hãng bảo vệ liên kết tóc.</li>
                    <li><strong>Bước 4:</strong> Xả dưỡng, khóa màu và sấy tạo kiểu bồng bềnh.</li>
                </ul>

                <p>Hãy liên hệ hoặc đặt lịch ngay với đội ngũ Master Stylist của MC Hair Salon để sở hữu màu tóc ấn tượng nhất mùa này!</p>
                """
            },
            new BlogPost
            {
                Title = "Quy Trình Phục Hồi Tóc Hư Tổn Chuyên Sâu Cùng Olaplex & B3",
                Slug = "quy-trinh-phuc-hoi-toc-hu-ton-chuyen-sau-olaplex-b3",
                Category = "Chăm sóc & Phục hồi",
                Summary = "Giải pháp tái tạo cấu trúc liên kết tóc đứt gãy sau nhiều lần tẩy nhuộm, giúp tóc bóng mượt và chắc khỏe từ gốc đến ngọn.",
                ThumbnailUrl = "/resources/dich_vu/d75f3d172426499f94aca1bb02c424cd.jpeg",
                AuthorName = "Master Ken",
                PublishedAt = DateTime.UtcNow.AddDays(-5),
                IsPublished = true,
                IsFeatured = true,
                ViewCount = 98,
                MetaTitle = "Phục Hồi Tóc Hư Tổn Chuyên Sâu Cùng Olaplex & B3 | MC Hair",
                MetaDescription = "Tìm hiểu liệu trình phục hồi tóc hư tổn nặng bằng Olaplex No.1, No.2 và B3 Brazilian Bond Builder độc quyền tại MC Hair Salon.",
                Content = """
                <p class="lead">Tóc khô xơ, chẻ ngọn hoặc gãy rụng sau quá trình uốn, duỗi, tẩy nhuộm liên tục là nỗi lo lắng của rất nhiều chị em. Liệu trình phục hồi đa tầng cùng Olaplex và B3 Bond Builder chính là 'thần dược' tái sinh mái tóc hư tổn nặng.</p>

                <h2>Tại sao phục hồi liên kết tóc lại quan trọng?</h2>
                <p>Nhiệt độ cao và hóa chất làm đứt gãy các cầu nối lưu huỳnh (Disulfide bonds) bên trong tủy tóc, khiến tóc mất đi độ đàn hồi và trở nên xốp, dễ đứt gãy. Các loại dầu xả thông thường chỉ phủ bóng tạm thời bề mặt sợi tóc, trong khi Olaplex trực tiếp hàn gắn các liên kết đứt gãy từ bên trong lõi tóc.</p>

                <h2>4 Bước Phục Hồi Chuyên Sâu Tại MC Hair</h2>
                <ol>
                    <li><strong>Gội thanh tẩy chuyên sâu:</strong> Loại bỏ tạp chất, kim loại nặng và cặn hóa chất bám trên biểu bì tóc.</li>
                    <li><strong>Nạp tinh chất Olaplex No.1 Bond Multiplier:</strong> Thẩm thấu sâu vào lõi tóc, tái tạo và hàn gắn các liên kết bị tổn thương.</li>
                    <li><strong>Ủ dưỡng khóa ẩm Olaplex No.2 & B3:</strong> Phục hồi lớp màng lipid bên ngoài, cung cấp axit amin và độ ẩm tinh khiết.</li>
                    <li><strong>Massage da đầu & tráng dưỡng lạnh:</strong> Khép chặt biểu bì tóc, khóa dưỡng chất và mang lại cảm giác thư giãn tuyệt đối.</li>
                </ol>

                <p>Sau liệu trình 60 phút, bạn sẽ cảm nhận rõ độ đanh chắc, mềm mượt và bóng khỏe tự nhiên của từng sợi tóc mà không hề bị nặng hay bết dính.</p>
                """
            },
            new BlogPost
            {
                Title = "Bí Quyết Giữ Nếp Tóc Uốn Sóng Lơi Bồng Bềnh Tại Nhà",
                Slug = "bi-quyet-giu-nep-toc-uon-song-loi-bong-benh-tai-nha",
                Category = "Kiến thức tóc",
                Summary = "Hướng dẫn chi tiết cách sấy tạo kiểu, chọn tinh dầu dưỡng và chăm sóc tóc uốn giữ lọn chuẩn salon suốt cả tuần.",
                ThumbnailUrl = "/resources/dich_vu/1536719a788647fea91b2bce5b89633d.jpeg",
                AuthorName = "MC Hair Team",
                PublishedAt = DateTime.UtcNow.AddDays(-8),
                IsPublished = true,
                IsFeatured = false,
                ViewCount = 76,
                MetaTitle = "Bí Quyết Giữ Nếp Tóc Uốn Sóng Lơi Bồng Bềnh Tại Nhà | MC Hair",
                MetaDescription = "Mẹo hay giúp giữ nếp tóc uốn xoăn sóng lơi tự nhiên tại nhà cực đơn giản từ các chuyên gia tạo mẫu tóc MC Hair.",
                Content = """
                <p class="lead">Tóc uốn sóng lơi Hàn Quốc luôn là lựa chọn hàng đầu nhờ vẻ đẹp tự nhiên, trẻ trung. Tuy nhiên, để duy trì lọn sóng luôn bồng bềnh như lúc vừa rời salon, bạn cần nắm vững những bí quyết chăm sóc đơn giản sau.</p>

                <h2>1. Kỹ thuật sấy ngón tay xoắn lọn</h2>
                <p>Sau khi gội đầu, hãy thấm khô tóc bằng khăn mềm (không chà xát mạnh). Khi sấy tóc đạt độ khô khoảng 70%, dùng ngón tay xoắn từng lọn tóc hướng ra sau hoặc vào trong theo nếp uốn, kết hợp sấy nhiệt ấm để định hình lọn tóc.</p>

                <h2>2. Sử dụng kẹp càng cua đúng cách</h2>
                <p>Trước khi đi ngủ hoặc khi ở nhà, hãy gom tóc xoắn nhẹ lại rồi cố định bằng kẹp càng cua trên đỉnh đầu. Cách làm này vừa giúp tóc không bị đè gãy nếp khi ngủ vừa tạo độ phồng chân tóc tự nhiên.</p>

                <h2>3. Luôn thoa tinh dầu dưỡng trước và sau khi sấy</h2>
                <p>Tinh dầu dưỡng tóc (như Moroccanoil Treatment) tạo lớp màng bảo vệ tóc khỏi nhiệt độ máy sấy và giúp các lọn xoăn bóng bẩy, đàn hồi tốt hơn.</p>
                """
            },
            new BlogPost
            {
                Title = "Cách Chọn Kiểu Tóc Layer Phù Hợp Với Từng Dáng Khuôn Mặt",
                Slug = "cach-chon-kieu-toc-layer-phu-hop-voi-tung-dang-khuon-mat",
                Category = "Xu hướng tóc",
                Summary = "Gợi ý những kiểu cắt tỉa layer tầng bay bổng giúp che khuyết điểm gò má, tôn lên đường nét thanh thoát của gương mặt.",
                ThumbnailUrl = "/resources/dich_vu/cat_toc.jpg",
                AuthorName = "Stylist Minh",
                PublishedAt = DateTime.UtcNow.AddDays(-12),
                IsPublished = true,
                IsFeatured = false,
                ViewCount = 115,
                MetaTitle = "Cách Chọn Kiểu Tóc Layer Phù Hợp Cho Từng Dáng Mặt | MC Hair",
                MetaDescription = "Tư vấn chọn dáng tóc tỉa layer cho mặt tròn, mặt vuông, mặt dài giúp thon gọn gương mặt và tạo vẻ đẹp thanh lịch.",
                Content = """
                <p class="lead">Kiểu cắt tỉa layer tầng xếp lớp là 'vũ khí' lợi hại giúp thon gọn khuôn mặt và tăng độ phồng tự nhiên cho mái tóc. Cùng MC Hair khám phá kiểu layer phù hợp nhất với bạn nhé!</p>

                <h2>1. Khuôn mặt tròn: Layer dài ngang lưng kết hợp mái bay</h2>
                <p>Các tầng layer so le ôm nhẹ hai bên xương hàm kết hợp mái bay bồng bềnh giúp kéo dài tỉ lệ khuôn mặt, mang lại cảm giác thanh thoát, thon gọn.</p>

                <h2>2. Khuôn mặt vuông góc cạnh: Layer sóng lơi mềm mại</h2>
                <p>Những lọn tóc layer được uốn sóng lơi nhẹ nhàng sẽ làm mềm các đường nét góc cạnh ở quai hàm, tạo vẻ nữ tính và quyến rũ.</p>

                <h2>3. Khuôn mặt dài: Layer ngang vai (Wolf Cut hoặc Shag Layer)</h2>
                <p>Tạo độ phồng ngang ở hai bên thái dương và kết hợp mái thưa hoặc mái ngố giúp cân bằng chiều dài khuôn mặt một cách hoàn hảo.</p>
                """
            }
        ];

        foreach (BlogPost post in posts)
        {
            await repository.AddBlogPostAsync(post, ct);
        }
    }

    private static async Task EnsureSchemaUpdatesAsync(SalonDbContext db, CancellationToken ct)
    {
        try
        {
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE salon_promotions ADD COLUMN StartDate TEXT;", ct);
        }
        catch
        {
            // Ignore if column already exists
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE salon_promotions ADD COLUMN EndDate TEXT;", ct);
        }
        catch
        {
            // Ignore if column already exists
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE salon_admin_users ADD COLUMN Role INTEGER NOT NULL DEFAULT 1;", ct);
        }
        catch
        {
            // Ignore if column already exists
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE salon_admin_users ADD COLUMN StylistId INTEGER;", ct);
        }
        catch
        {
            // Ignore if column already exists
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE salon_blog_posts ADD COLUMN VideoUrl TEXT;", ct);
        }
        catch
        {
            // Ignore if column already exists
        }

        try
        {
            BlogPost? existingVideoPost = await db.BlogPosts.FirstOrDefaultAsync(
                x => x.Slug == "video-trai-nghiem-quy-trinh-tao-mau-nhuom-toc-thiet-ke-mc-hair-salon",
                ct
            );
            if (existingVideoPost is null)
            {
                db.BlogPosts.Add(new BlogPost
                {
                    Title = "[Video] Trải Nghiệm Quy Trình Tạo Mẫu & Nhuộm Tóc Thiết Kế Tại MC Hair Salon",
                    Slug = "video-trai-nghiem-quy-trinh-tao-mau-nhuom-toc-thiet-ke-mc-hair-salon",
                    Category = "Xu hướng tóc",
                    Summary = "Video cận cảnh quy trình tư vấn 1:1, kỹ thuật nhuộm chuyển sắc Balayage và chăm sóc phục hồi chuyên sâu tại kênh YouTube @mchairsalon.",
                    ThumbnailUrl = "/resources/dich_vu/997d6f7b2107436a9338c7f6ec2547cb.jpeg",
                    VideoUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                    AuthorName = "MC Hair Team",
                    PublishedAt = DateTime.UtcNow,
                    IsPublished = true,
                    IsFeatured = true,
                    ViewCount = 238,
                    MetaTitle = "Trải Nghiệm Thực Tế Tại MC Hair Salon | Kênh YouTube @mchairsalon",
                    MetaDescription = "Xem video thực tế quy trình làm tóc chuyên nghiệp tại MC Hair Salon và đăng ký theo dõi kênh YouTube @mchairsalon.",
                    Content = """
                    <p class="lead">Chào mừng bạn đến với kênh chính thức của <strong>MC Hair Salon</strong>! Trong video dưới đây, hãy cùng theo chân đội ngũ Stylist khám phá quy trình biến hình mái tóc từ khâu kiểm tra chất tóc, tư vấn phối màu cho đến bước tạo kiểu hoàn thiện đầy ấn tượng.</p>

                    <h2>Khám Phá Kỹ Thuật Tạo Mẫu Độc Bản Tại MC Hair</h2>
                    <p>Mỗi mái tóc tại MC Hair đều là một tác phẩm nghệ thuật cá nhân hóa, được thiết kế tỉ mỉ dựa trên cấu trúc gương mặt, phong cách và chất tóc riêng của từng khách hàng.</p>

                    <div class="embedded-video-wrapper">
                        <iframe src="https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ" allowfullscreen loading="lazy" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"></iframe>
                    </div>

                    <h2>Những Điểm Nổi Bật Trong Quy Trình Của Chúng Tôi:</h2>
                    <ul>
                        <li><strong>Tư vấn 1:1 chuyên sâu:</strong> Lắng nghe mong muốn và phân tích cấu trúc sợi tóc kỹ lưỡng trước khi bắt đầu.</li>
                        <li><strong>Sản phẩm cao cấp:</strong> Sử dụng 100% dòng mỹ phẩm tóc hàng đầu thế giới (Olaplex, L'Oréal Professionnel, Moroccanoil).</li>
                        <li><strong>Kỹ thuật điêu luyện:</strong> Đội ngũ Stylist nhiều năm kinh nghiệm, liên tục cập nhật các xu hướng tóc quốc tế hot nhất.</li>
                        <li><strong>Chế độ bảo hành tận tâm:</strong> Cam kết đồng hành và chăm sóc mái tóc khách hàng sau dịch vụ.</li>
                    </ul>

                    <blockquote>"Đừng quên nhấn Đăng ký (Subscribe) kênh YouTube chính thức của chúng tôi tại <a href='https://www.youtube.com/@mchairsalon' target='_blank' rel='noopener'>@mchairsalon</a> để cập nhật những video chia sẻ mẹo làm đẹp và các kiểu tóc mới nhất nhé!"</blockquote>
                    """
                });
                await db.SaveChangesAsync(ct);
            }
        }
        catch
        {
            // Ignore if error during seed
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
