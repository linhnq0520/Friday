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
            await db.Database.ExecuteSqlRawAsync("ALTER TABLE salon_blog_posts ADD COLUMN SortOrder INTEGER NOT NULL DEFAULT 0;", ct);
        }
        catch
        {
            // Ignore if column already exists
        }

        string[] serviceColumns = [
            "ALTER TABLE salon_services ADD COLUMN Slug TEXT;",
            "ALTER TABLE salon_services ADD COLUMN Headline TEXT;",
            "ALTER TABLE salon_services ADD COLUMN ShortDescription TEXT;",
            "ALTER TABLE salon_services ADD COLUMN PriceTagText TEXT;",
            "ALTER TABLE salon_services ADD COLUMN DurationText TEXT;",
            "ALTER TABLE salon_services ADD COLUMN BadgeText TEXT;",
            "ALTER TABLE salon_services ADD COLUMN HeroImageUrl TEXT;",
            "ALTER TABLE salon_services ADD COLUMN BeforeImageUrl TEXT;",
            "ALTER TABLE salon_services ADD COLUMN AfterImageUrl TEXT;",
            "ALTER TABLE salon_services ADD COLUMN PricingTableJson TEXT;",
            "ALTER TABLE salon_services ADD COLUMN MethodsJson TEXT;",
            "ALTER TABLE salon_services ADD COLUMN StepsJson TEXT;",
            "ALTER TABLE salon_services ADD COLUMN FaqsJson TEXT;",
            "ALTER TABLE salon_services ADD COLUMN ContentHtml TEXT;"
        ];
        foreach (string sql in serviceColumns)
        {
            try
            {
                await db.Database.ExecuteSqlRawAsync(sql, ct);
            }
            catch
            {
                // Ignore if column already exists
            }
        }

        try
        {
            await db.Database.ExecuteSqlRawAsync("UPDATE salon_services SET Slug = 'dich-vu-' || Id WHERE Slug IS NULL OR Slug = '';", ct);
        }
        catch
        {
            // Ignore if error updating null slugs
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

    public static async Task EnsureServiceDetailsSeededAsync(
        IServiceProvider services,
        CancellationToken ct = default
    )
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        ISalonRepository repository = scope.ServiceProvider.GetRequiredService<ISalonRepository>();
        SalonDbContext db = scope.ServiceProvider.GetRequiredService<SalonDbContext>();

        IReadOnlyList<HairService> servicesList = await repository.GetAllServicesAsync(ct);
        if (servicesList.Count == 0)
        {
            return;
        }

        bool changed = false;

        foreach (HairService s in servicesList)
        {
            if (!string.IsNullOrWhiteSpace(s.Slug) && !string.IsNullOrWhiteSpace(s.Headline))
            {
                continue;
            }

            string normalizedName = s.Name.Trim().ToLowerInvariant();
            if (normalizedName.Contains("nối") || normalizedName.Contains("noi"))
            {
                s.Slug = "noi-toc";
                s.Headline = "Nối tóc dài tự nhiên, mối nối gọn khó nhận ra";
                s.ShortDescription = "Tóc dài, dày tự nhiên trong 2-3h với kỹ thuật không đau, không lộ mối nối";
                s.Description = "Nối keo, nối light, nối lông vũ — MC Hair Salon tư vấn phương pháp phù hợp với tình trạng tóc thật của bạn, mối nối nhỏ gọn tự nhiên, hướng dẫn chăm sóc để tóc nối bền đẹp.";
                s.PriceTagText = "Từ 16K/tép";
                s.DurationText = "2–3 giờ";
                s.BadgeText = "Tìm nhiều nhất";
                s.ImageUrl ??= "/resources/dich_vu/noi_toc.jpg";
                s.HeroImageUrl = "/resources/bo_suu_tap/noi_toc/eac43f0bcf694829bc1a822034ab0937.png";
                s.BeforeImageUrl = "/resources/before_after/5c14189be5f248a0a65c847d169a0c6c.jpg";
                s.AfterImageUrl = "/resources/before_after/69c14f69b5b144428ed075bbd48823f8.jpg";
                s.PricingTableJson = """
                {
                    "Groups": [
                        {
                            "Title": "Theo độ dài tóc nối (giá / tép)",
                            "Columns": ["", "Loại 1", "VIP"],
                            "Items": [
                                { "Name": "45cm", "Price1": "16K/tép", "Price2": "19K/tép", "Note": "" },
                                { "Name": "55cm", "Price1": "19K/tép", "Price2": "22K/tép", "Note": "" },
                                { "Name": "60cm", "Price1": "22K/tép", "Price2": "25K/tép", "Note": "" },
                                { "Name": "Tóc tẩy 50–60cm", "Price1": "", "Price2": "28K/tép", "Note": "Tẩy lên level 9-10" },
                                { "Name": "Tóc Balayage", "Price1": "", "Price2": "32K/tép", "Note": "Hiệu ứng Balayage đã bao gồm lên màu" }
                            ]
                        },
                        {
                            "Title": "Theo phương pháp nối",
                            "Columns": ["", "S", "M", "L"],
                            "Items": [
                                { "Name": "Nối light (highlight)", "Price1": "60K", "Price2": "70K / 80K", "Note": "" },
                                { "Name": "Nối lông vũ", "Price1": "Từ 16K/tép", "Price2": "", "Note": "Mối siêu nhỏ, không đau" },
                                { "Name": "Nối tóc dài (trên 50cm)", "Price1": "Từ 19K/tép", "Price2": "", "Note": "Tóc thật 100%" }
                            ]
                        }
                    ],
                    "Addons": [
                        { "Name": "Nâng mối nối (bảo trì định kỳ)", "Price": "6K/tép" },
                        { "Name": "Tháo tóc nối chuyên dụng", "Price": "300K – 600K" },
                        { "Name": "Dưỡng phục hồi chuyên sâu tóc nối", "Price": "350K – 450K" },
                        { "Name": "Nối kèm nhuộm light thiết kế", "Price": "60K – 80K" }
                    ],
                    "Note": "Giá trên đã bao gồm tư vấn 1:1, kiểm tra chất tóc và bảo hành mối nối 30 ngày tại salon."
                }
                """;
                s.MethodsJson = """
                [
                    { "Title": "Nối lông vũ siêu vi", "Description": "Mối nối siêu nhỏ chỉ bằng hạt gạo, nhẹ êm, không cộm, buộc tóc cao hoàn toàn không lộ.", "Icon": "feather" },
                    { "Title": "Nối keo Fusion nhiệt", "Description": "Dùng keratin sinh học gắn từng lọn tỉ mỉ, mối chắc chắn, phù hợp nối dày cả đầu.", "Icon": "link" },
                    { "Title": "Nối light tạo điểm nhấn", "Description": "Kết hợp nối dài và tạo highlight đa sắc màu mà không cần tẩy tóc thật.", "Icon": "sparkles" },
                    { "Title": "Nối tóc dài VIP", "Description": "Nối thêm độ dài lớn (50-70cm) cho tóc ngắn, sử dụng 100% tóc thật loại 1 tuyển chọn.", "Icon": "maximize" },
                    { "Title": "Nối mái / dặm thưa", "Description": "Bổ sung phần tóc mái hoặc vùng thái dương thưa mỏng giúp gương mặt đầy đặn tự nhiên.", "Icon": "user" }
                ]
                """;
                s.StepsJson = """
                [
                    { "StepNumber": 1, "Title": "Kiểm tra chất tóc & Tư vấn 1:1", "Description": "Đánh giá độ chắc khỏe của chân tóc thật và tư vấn kỹ thuật nối cũng như độ dài, mật độ phù hợp." },
                    { "StepNumber": 2, "Title": "Lựa chọn tép tóc chuẩn màu", "Description": "Tuyển chọn tóc nối tương đồng 100% về chất sợi, độ bóng và tone màu với tóc của bạn." },
                    { "StepNumber": 3, "Title": "Thực hiện nối tỉ mỉ", "Description": "Kỹ thuật viên tay nghề cao phân chia từng phân khu và trau chuốt từng mối nối đều tăm tắp." },
                    { "StepNumber": 4, "Title": "Cắt tỉa & Tạo kiểu hoàn thiện", "Description": "Tỉa layer chuyển tiếp giữa tóc thật và tóc nối để tạo hiệu ứng suôn mượt liền mạch tự nhiên." },
                    { "StepNumber": 5, "Title": "Hướng dẫn chăm sóc & Hẹn lịch nâng mối", "Description": "Chia sẻ bí quyết gội sấy, chải tóc tại nhà và hẹn lịch kiểm tra, nâng mối định kỳ 2-3 tháng." }
                ]
                """;
                s.FaqsJson = """
                [
                    { "Question": "Nối tóc bao nhiêu tiền?", "Answer": "Chi phí nối tóc tại MC Hair từ 16K/tép tùy theo độ dài tóc nối (45cm - 60cm) và kỹ thuật bạn chọn. Bạn có thể ghé salon để được báo giá chính xác theo lượng tóc cần nối." },
                    { "Question": "Nối tóc giữ được bao lâu?", "Answer": "Trung bình mối nối giữ chắc đẹp từ 3–6 tháng. Sau khoảng 2–3 tháng khi tóc thật dài ra, bạn chỉ cần quay lại salon nâng mối để giữ form đẹp nhất." },
                    { "Question": "Nối tóc có hại hoặc làm đứt gãy tóc thật không?", "Answer": "Hoàn toàn không nếu được thực hiện chuẩn kỹ thuật và chia đều trọng lượng tép tóc. MC Hair cam kết không giật chân tóc và sử dụng chất liệu kết nối an toàn cho da đầu." },
                    { "Question": "Nối light tóc bao nhiêu tiền?", "Answer": "Nối light chỉ từ 60K - 80K/tép đã bao gồm màu nhuộm thời trang (khói, hồng đào, bạch kim...) không cần tẩy tóc thật." },
                    { "Question": "Thời gian hoàn thành một bộ tóc nối mất bao lâu?", "Answer": "Thông thường mất khoảng 2 – 3.5 giờ tùy theo số lượng tép nối (nối dặm, nối nửa đầu hoặc nối dày cả đầu)." }
                ]
                """;
                changed = true;
            }
            else if (normalizedName.Contains("uốn") || normalizedName.Contains("uon"))
            {
                s.Slug = "uon-toc";
                s.Headline = "Uốn tóc bồng bềnh chuẩn nếp, mềm mượt tự nhiên";
                s.ShortDescription = "Uốn xoăn layer, sóng lơi, cúp phồng giữ nếp 3-6 tháng với thuốc cao cấp";
                s.Description = "MC Hair Salon ứng dụng công nghệ uốn định hình tiên tiến bảo vệ sợi tóc, tư vấn sóng xoăn phù hợp dáng mặt, đảm bảo lọn sóng bồng bềnh tự nhiên và dễ chăm sóc tại nhà.";
                s.PriceTagText = "Từ 650K";
                s.DurationText = "2–3 giờ";
                s.BadgeText = "Ưa chuộng";
                s.ImageUrl ??= "/resources/dich_vu/uon_duoi.jpg";
                s.HeroImageUrl = "/resources/bo_suu_tap/kieu_toc_thinh_hanh/3ce69e1269214dc9b860fc93da1a7f7b.jpg";
                s.BeforeImageUrl = "/resources/before_after/2598f3a1d9a243d89d1af6eff0ffa8ef.jpg";
                s.AfterImageUrl = "/resources/before_after/2d074cb0bbf24092a662cc4ba485bb7e.jpg";
                s.PricingTableJson = """
                {
                    "Groups": [
                        {
                            "Title": "Bảng giá uốn tóc theo độ dài & dòng thuốc",
                            "Columns": ["Dịch vụ", "Tiêu chuẩn", "Cao cấp L'Oréal"],
                            "Items": [
                                { "Name": "Uốn tóc ngắn (Size S)", "Price1": "650.000đ", "Price2": "950.000đ", "Note": "Tóc ngang cằm / bob" },
                                { "Name": "Uốn tóc lửng (Size M)", "Price1": "850.000đ", "Price2": "1.250.000đ", "Note": "Tóc chạm vai / xương quai xanh" },
                                { "Name": "Uốn tóc dài (Size L)", "Price1": "1.050.000đ", "Price2": "1.550.000đ", "Note": "Tóc ngang ngực trở xuống" },
                                { "Name": "Uốn phục hồi Collagen/Keratin", "Price1": "1.200.000đ", "Price2": "1.800.000đ", "Note": "Dành riêng tóc hư tổn / tẩy" }
                            ]
                        }
                    ],
                    "Addons": [
                        { "Name": "Cắt tạo form layer trước khi uốn", "Price": "Miễn phí" },
                        { "Name": "Hấp phục hồi khóa nếp Olaplex", "Price": "350.000đ" },
                        { "Name": "Uốn phồng chân tóc không lộ mối", "Price": "300.000đ" }
                    ],
                    "Note": "Cam kết bảo hành nếp uốn trong vòng 15 ngày, hỗ trợ chỉnh sửa miễn phí nếu chưa vừa ý."
                }
                """;
                s.MethodsJson = """
                [
                    { "Title": "Uốn sóng lơi Hàn Quốc", "Description": "Tạo lọn xoăn tự nhiên, nhẹ nhàng như gió thoảng, mang lại vẻ đẹp thanh lịch nữ tính.", "Icon": "wind" },
                    { "Title": "Uốn xoăn Layer bồng bềnh", "Description": "Từng lớp tóc so le ôm trọn khung xương hàm, tạo hiệu ứng tóc dày và bồng bềnh gấp đôi.", "Icon": "layers" },
                    { "Title": "Uốn cúp chữ C / J", "Description": "Phần ngọn tóc cụp nhẹ ôm lấy gương mặt, phong cách trẻ trung, năng động và thanh thoát.", "Icon": "smile" },
                    { "Title": "Uốn phồng chân tóc chân ái", "Description": "Khắc phục triệt để tình trạng tóc xẹp dính da đầu, tạo độ phồng tự nhiên cả ngày.", "Icon": "sparkles" }
                ]
                """;
                s.StepsJson = """
                [
                    { "StepNumber": 1, "Title": "Tư vấn dáng sóng theo khuôn mặt", "Description": "Stylist xem xét chất tóc và hình dáng khuôn mặt để chọn kích thước trục uốn lý tưởng." },
                    { "StepNumber": 2, "Title": "Cắt tạo tầng Layer chuẩn tỉ lệ", "Description": "Tỉa phom tóc tạo độ so le chuẩn xác để khi uốn các lọn tóc bung lớp tự nhiên." },
                    { "StepNumber": 3, "Title": "Vào thuốc định hình & Chạy nhiệt kỹ thuật số", "Description": "Sử dụng dòng thuốc uốn giàu dưỡng chất và máy uốn setting kiểm soát nhiệt độ an toàn." },
                    { "StepNumber": 4, "Title": "Dập định hình sóng & Xả dưỡng", "Description": "Khóa form lọn xoăn bền lâu và bổ sung tinh chất dưỡng ẩm sâu cho sợi tóc bóng khỏe." },
                    { "StepNumber": 5, "Title": "Sấy tạo kiểu & Hướng dẫn quấn tay tại nhà", "Description": "Hướng dẫn chi tiết thao tác sấy khô và quấn tay đơn giản để bạn tự chăm sóc ở nhà 5 phút mỗi ngày." }
                ]
                """;
                s.FaqsJson = """
                [
                    { "Question": "Uốn tóc tại MC Hair giữ nếp được bao lâu?", "Answer": "Thông thường nếp uốn giữ đẹp từ 3–6 tháng hoặc lâu hơn tùy cơ địa sợi tóc và cách bạn chăm sóc." },
                    { "Question": "Tóc từng tẩy hoặc yếu có uốn được không?", "Answer": "Stylist sẽ test độ đàn hồi tóc trước. Với tóc yếu, MC Hair áp dụng liệu trình bọc dưỡng phục hồi trước khi vào thuốc để đảm bảo an toàn cho mái tóc." },
                    { "Question": "Sau khi uốn về nhà có khó chăm sóc không?", "Answer": "Rất đơn giản, stylist sẽ hướng dẫn bạn cách quấn tay và sấy khô tự nhiên mà không cần dùng lô cuốn phức tạp." }
                ]
                """;
                changed = true;
            }
            else if (normalizedName.Contains("nhuộm") || normalizedName.Contains("nhuom"))
            {
                s.Slug = "nhuom-toc";
                s.Headline = "Nhuộm màu chuẩn sắc hot trend, bền màu sáng bóng";
                s.ShortDescription = "Nhuộm màu thời trang, Balayage, Ombre bền màu lâu với thuốc nhuộm cao cấp L'Oréal";
                s.Description = "MC Hair Salon chuyên các tone màu thời thượng: nâu trà sữa, xám khói, beige tây, kỹ thuật Balayage chuyển sắc mượt mà không lo lộ chân tóc khi mọc dài.";
                s.PriceTagText = "Từ 650K";
                s.DurationText = "1.5–2.5 giờ";
                s.BadgeText = "Phổ biến nhất";
                s.ImageUrl ??= "/resources/dich_vu/nhuom_tay.jpg";
                s.HeroImageUrl = "/resources/bo_suu_tap/mau_thoi_trang/744ee6efea71440cb9fbd1bed3210abc.jpg";
                s.BeforeImageUrl = "/resources/before_after/73f20300051c4ea683b1e449bd39d348.jpg";
                s.AfterImageUrl = "/resources/before_after/74a48592d0854e49a8fa934bb703064d.jpg";
                s.PricingTableJson = """
                {
                    "Groups": [
                        {
                            "Title": "Bảng giá nhuộm tóc thời trang",
                            "Columns": ["Dịch vụ", "Tiêu chuẩn", "Cao cấp L'Oréal"],
                            "Items": [
                                { "Name": "Nhuộm tóc ngắn (Size S)", "Price1": "650.000đ", "Price2": "900.000đ", "Note": "Tone màu thời trang tôn da" },
                                { "Name": "Nhuộm tóc lửng (Size M)", "Price1": "850.000đ", "Price2": "1.200.000đ", "Note": "Đồng đều màu từ chân đến ngọn" },
                                { "Name": "Nhuộm tóc dài (Size L)", "Price1": "1.050.000đ", "Price2": "1.500.000đ", "Note": "Bao phủ dưỡng bóng mềm" },
                                { "Name": "Nhuộm thiết kế Balayage / Ombre", "Price1": "1.500.000đ", "Price2": "2.500.000đ", "Note": "Kỹ thuật cọ vẽ chuyển sắc độc bản" },
                                { "Name": "Tẩy tóc / Nâng tone an toàn", "Price1": "300.000đ / lần", "Price2": "450.000đ / lần", "Note": "Thuốc tẩy bảo vệ tủy tóc Olaplex" }
                            ]
                        }
                    ],
                    "Addons": [
                        { "Name": "Bọc dưỡng Olaplex số 1 & 2 khi nhuộm", "Price": "350.000đ" },
                        { "Name": "Khóa màu phủ bóng công nghệ Nano", "Price": "250.000đ" }
                    ],
                    "Note": "Tất cả sản phẩm nhuộm nhập khẩu chính hãng L'Oréal, Moroccanoil, đảm bảo không rát da đầu."
                }
                """;
                s.MethodsJson = """
                [
                    { "Title": "Nhuộm Balayage nghệ thuật", "Description": "Kỹ thuật quét cọ Pháp tạo độ loang tự nhiên, chân tóc sẫm màu mọc dài ra vẫn giữ nét đẹp sang trọng.", "Icon": "brush" },
                    { "Title": "Nhuộm Highlight / Babylights", "Description": "Các dải sợi sáng mảnh xen kẽ mái tóc tạo độ sâu và lấp lánh khi có ánh nắng phản chiếu.", "Icon": "sun" },
                    { "Title": "Nhuộm tone Tây không tẩy", "Description": "Các gam màu nâu tây, nâu lạnh, nâu hạt dẻ sáng bóng tôn sáng làn da mà không cần tẩy hại tóc.", "Icon": "heart" },
                    { "Title": "Nhuộm thời trang khói / pastel", "Description": "Sở hữu các màu trendy như xám khói, trà sữa, hồng trà với công thức giữ hạt màu độc quyền.", "Icon": "palette" }
                ]
                """;
                s.StepsJson = """
                [
                    { "StepNumber": 1, "Title": "Tư vấn màu sắc theo sắc tố da (Personal Color)", "Description": "Stylist giúp bạn chọn tone màu phù hợp nhất với màu da và phong cách cá nhân." },
                    { "StepNumber": 2, "Title": "Bảo vệ da đầu & Bọc dưỡng", "Description": "Xịt tinh chất chống rát da đầu và bổ sung tinh chất bảo vệ biểu bì tóc." },
                    { "StepNumber": 3, "Title": "Pha màu chuẩn sắc & Tiến hành nhuộm", "Description": "Cân đo tỉ lệ thuốc nhuộm chuẩn xác và thao tác nhuộm đều tay từng lớp tóc." },
                    { "StepNumber": 4, "Title": "Khử kiềm, Khóa màu & Phục hồi", "Description": "Gội xả chuyên biệt khóa hạt màu sâu trong lõi tóc giúp màu bền lâu và sáng bóng." },
                    { "StepNumber": 5, "Title": "Sấy tạo kiểu & Hướng dẫn dưỡng màu tại nhà", "Description": "Tạo kiểu nhẹ nhàng và tư vấn dầu gội tím/dưỡng màu phù hợp cho tóc nhuộm." }
                ]
                """;
                s.FaqsJson = """
                [
                    { "Question": "Nhuộm tóc có cần tẩy không?", "Answer": "Phụ thuộc vào màu tóc bạn muốn. Các màu tone nâu tây, nâu lạnh, chocolate không cần tẩy. Các màu pastel, xám khói, rêu sáng cần tẩy nhẹ." },
                    { "Question": "Nhuộm tóc tại MC Hair có bị rát da đầu không?", "Answer": "MC Hair luôn sử dụng thuốc nhuộm chất lượng cao kèm xịt bảo vệ màng da đầu chuyên dụng nên bạn hoàn toàn yên tâm êm dịu." },
                    { "Question": "Nhuộm Balayage có bền màu hơn nhuộm thông thường không?", "Answer": "Balayage có ưu điểm vượt trội là khi chân tóc thật mọc dài ra trông vẫn tự nhiên, bạn có thể để 6-9 tháng mới cần dặm lại." }
                ]
                """;
                changed = true;
            }
            else if (normalizedName.Contains("cắt") || normalizedName.Contains("cat"))
            {
                s.Slug = "cat-toc-nu";
                s.Headline = "Cắt tóc tỉa layer chuẩn form, tôn đường nét gương mặt";
                s.ShortDescription = "Cắt layer tầng, tỉa mái bay, tạo form tóc bồng bềnh phù hợp từng cá nhân";
                s.Description = "MC Hair Salon tư vấn 1:1 theo tỉ lệ gương mặt, thiết kế các phom tóc Layer, Bob, Pixie thời thượng, che khuyết điểm gò má hay cằm thô một cách hoàn hảo.";
                s.PriceTagText = "Từ 150K";
                s.DurationText = "30–60 phút";
                s.BadgeText = "Thiết kế 1:1";
                s.ImageUrl ??= "/resources/dich_vu/cat_toc.jpg";
                s.HeroImageUrl = "/resources/bo_suu_tap/kieu_toc_thinh_hanh/6ec194de5a3b4f80806342288ed06a3a.jpg";
                s.BeforeImageUrl = "/resources/before_after/968f8796dca943abb3ecc66044837a05.jpg";
                s.AfterImageUrl = "/resources/before_after/b210fb159e384341ae8271731f6aaf47.jpg";
                s.PricingTableJson = """
                {
                    "Groups": [
                        {
                            "Title": "Bảng giá cắt tạo kiểu tóc nữ",
                            "Columns": ["Hạng mục", "Stylist", "Master Art"],
                            "Items": [
                                { "Name": "Cắt tạo kiểu tóc nữ cơ bản", "Price1": "150.000đ", "Price2": "250.000đ", "Note": "Bao gồm gội sấy cơ bản" },
                                { "Name": "Cắt thiết kế Layer / Wolf Cut", "Price1": "200.000đ", "Price2": "300.000đ", "Note": "Tỉa đa tầng tạo độ bay" },
                                { "Name": "Cắt tóc ngắn Pixie / Bob cá tính", "Price1": "200.000đ", "Price2": "350.000đ", "Note": "Căn chỉnh tỉ lệ cằm & gáy" },
                                { "Name": "Cắt tỉa mái bay / mái thưa Hàn Quốc", "Price1": "50.000đ", "Price2": "80.000đ", "Note": "Tạo đường cong che gò má" }
                            ]
                        }
                    ],
                    "Addons": [
                        { "Name": "Gội massage thư giãn tinh dầu thảo mộc", "Price": "100.000đ" },
                        { "Name": "Sấy tạo phom sóng bay dự tiệc", "Price": "120.000đ" }
                    ],
                    "Note": "Đã bao gồm gội xả thư giãn và sấy tạo kiểu hoàn thiện."
                }
                """;
                s.MethodsJson = """
                [
                    { "Title": "Cắt Layer bay bổng", "Description": "Từng lớp tóc so le tạo cảm giác thanh thoát, tự ôm cúp tự nhiên sau khi gội đầu.", "Icon": "scissors" },
                    { "Title": "Mái bay Hàn Quốc tôn đường nét", "Description": "Độ dài mái ôm nhẹ xương gò má, che gò má cao và góc hàm thô hiệu quả diệu kỳ.", "Icon": "sparkles" },
                    { "Title": "Short Bob & Pixie thời thượng", "Description": "Khoe trọn cần cổ thon gọn và góc nghiêng quyến rũ, trẻ trung và năng động.", "Icon": "star" }
                ]
                """;
                s.StepsJson = """
                [
                    { "StepNumber": 1, "Title": "Lắng nghe mong muốn & Tư vấn khuôn mặt", "Description": "Trao đổi kỹ lưỡng để chọn kiểu tóc vừa đẹp vừa hợp tính chất công việc của bạn." },
                    { "StepNumber": 2, "Title": "Gội xả làm sạch & Thư giãn", "Description": "Làm sạch tóc và da đầu bằng dầu gội cao cấp giúp tóc ẩm mềm chuẩn bị cắt." },
                    { "StepNumber": 3, "Title": "Cắt tạo cấu trúc phom dáng", "Description": "Hair Artist cắt định hình phom và tỉa tầng chuẩn xác theo từng góc độ." },
                    { "StepNumber": 4, "Title": "Sấy khô & Tỉa chi tiết (Dry Cut)", "Description": "Kiểm tra độ rơi tự nhiên của tóc khi khô để tỉa lại những sợi tóc chưa mượt mà." },
                    { "StepNumber": 5, "Title": "Tạo kiểu & Hướng dẫn sấy tại nhà", "Description": "Sấy phồng bồng bềnh và hướng dẫn cách giữ form đơn giản khi ở nhà." }
                ]
                """;
                s.FaqsJson = """
                [
                    { "Question": "Cắt tóc layer có cần phải sấy chải nhiều không?", "Answer": "MC Hair cắt theo nếp rơi tự nhiên của sợi tóc nên khi gội xong bạn chỉ cần sấy khô là tóc đã tự động vào nếp đẹp." },
                    { "Question": "Mặt tròn thì cắt kiểu nào hợp nhất?", "Answer": "Tóc layer ngang vai kết hợp mái bay dài ôm sát má là lựa chọn hoàn hảo giúp mặt bạn trông thon gọn hơn đáng kể." }
                ]
                """;
                changed = true;
            }
            else if (normalizedName.Contains("duỗi") || normalizedName.Contains("duoi"))
            {
                s.Slug = "duoi-toc";
                s.Headline = "Duỗi tóc thẳng mượt tự nhiên, suôn mềm không đơ cứng";
                s.ShortDescription = "Duỗi thẳng tự nhiên, duỗi cúp ngọn với công nghệ ion nano siêu bóng";
                s.Description = "Công nghệ duỗi tóc thông minh tại MC Hair giúp giải quyết triệt để tình trạng tóc xù rối, mang lại mái tóc suôn mượt óng ả, mềm mại tự nhiên chứ không hề đơ cứng.";
                s.PriceTagText = "Từ 650K";
                s.DurationText = "2–3 giờ";
                s.BadgeText = "Mềm mượt";
                s.ImageUrl ??= "/resources/dich_vu/313954147c4b4a2084c5809db98a7c0c.png";
                s.HeroImageUrl = "/resources/bo_suu_tap/kieu_toc_thinh_hanh/88d779aa38264581a65c95dd0f752bff.jpeg";
                s.BeforeImageUrl = "/resources/before_after/4467bfd7e5434736851b9a5ce7a567d8.jpg";
                s.AfterImageUrl = "/resources/before_after/500f95c6900d42008d96839267d15bcf.jpg";
                s.PricingTableJson = """
                {
                    "Groups": [
                        {
                            "Title": "Bảng giá duỗi tóc suôn mượt",
                            "Columns": ["Dịch vụ", "Tiêu chuẩn", "Cao cấp L'Oréal"],
                            "Items": [
                                { "Name": "Duỗi tóc ngắn (Size S)", "Price1": "650.000đ", "Price2": "950.000đ", "Note": "Tóc tự nhiên không xù" },
                                { "Name": "Duỗi tóc lửng (Size M)", "Price1": "850.000đ", "Price2": "1.250.000đ", "Note": "Suôn mượt từ gốc tới ngọn" },
                                { "Name": "Duỗi tóc dài (Size L)", "Price1": "1.050.000đ", "Price2": "1.550.000đ", "Note": "Mềm mại như dải lụa" },
                                { "Name": "Duỗi cúp ngọn tự nhiên", "Price1": "950.000đ", "Price2": "1.400.000đ", "Note": "Thân thẳng mượt, ngọn cụp nhẹ" }
                            ]
                        }
                    ],
                    "Addons": [
                        { "Name": "Hấp phục hồi Collagen khóa ẩm", "Price": "300.000đ" },
                        { "Name": "Cắt tỉa phom tóc", "Price": "Miễn phí" }
                    ],
                    "Note": "Bảo hành thẳng mượt 30 ngày, cam kết không gãy nếp khi buộc."
                }
                """;
                s.MethodsJson = """
                [
                    { "Title": "Duỗi thẳng tự nhiên", "Description": "Giữ lại độ phồng tự nhiên của tóc, loại bỏ hoàn toàn cảm giác đơ cứng giả tạo.", "Icon": "activity" },
                    { "Title": "Duỗi cúp đuôi chữ C", "Description": "Thân tóc thẳng suôn mượt kết hợp phần đuôi cụp nhẹ nhàng ôm dáng vai.", "Icon": "check" },
                    { "Title": "Duỗi phục hồi Keratin", "Description": "Dành cho tóc hư tổn xù rối, nạp đầy protein giúp sợi tóc chắc khỏe và mượt mà.", "Icon": "shield" }
                ]
                """;
                s.StepsJson = """
                [
                    { "StepNumber": 1, "Title": "Khám tóc & Kiểm tra độ khỏe", "Description": "Xác định nồng độ thuốc duỗi tương ứng với từng đoạn thân và ngọn tóc." },
                    { "StepNumber": 2, "Title": "Bôi thuốc mềm hóa & Canh giãn nở", "Description": "Theo dõi sát sao từng phút để thuốc phát huy tác dụng mà không làm tổn thương biểu bì." },
                    { "StepNumber": 3, "Title": "Kẹp là nhiệt ion nano mịn màng", "Description": "Sử dụng máy kẹp nhiệt titan phủ ceramic là nhẹ nhàng từng lọn mỏng." },
                    { "StepNumber": 4, "Title": "Dập định hình liên kết sợi tóc", "Description": "Cố định phom thẳng vĩnh viễn và cấp ẩm bù nước cho tóc." },
                    { "StepNumber": 5, "Title": "Xả dưỡng & Sấy khô hoàn thiện", "Description": "Mái tóc suôn óng ả, mềm như tơ lụa ngay khi vừa hoàn thành." }
                ]
                """;
                s.FaqsJson = """
                [
                    { "Question": "Duỗi tóc xong bao lâu thì được gội đầu?", "Answer": "Sau 48 giờ bạn có thể gội đầu bình thường bằng dầu gội dịu nhẹ không chứa sulfate." },
                    { "Question": "Duỗi tóc có bị xẹp dính vào da đầu không?", "Answer": "Kỹ thuật duỗi cách chân tóc 1.5 - 2cm tại MC Hair giúp tóc thẳng mượt mà vẫn giữ được độ phồng bồng bềnh." }
                ]
                """;
                changed = true;
            }
            else if (normalizedName.Contains("phục hồi") || normalizedName.Contains("phuc hoi") || normalizedName.Contains("olaplex"))
            {
                s.Slug = "phuc-hoi-toc";
                s.Headline = "Phục hồi hư tổn chuyên sâu đa tầng Olaplex & Keratin";
                s.ShortDescription = "Tái tạo liên kết tóc đứt gãy, trả lại mái tóc chắc khỏe đàn hồi ngay sau 1 liệu trình";
                s.Description = "Liệu trình phục hồi độc quyền với sản phẩm chính hãng Olaplex số 1 & 2 từ Mỹ, hàn gắn các liên kết lưu huỳnh bị đứt gãy do hóa chất, mang lại sức sống mới cho mái tóc tưởng chừng phải cắt bỏ.";
                s.PriceTagText = "Từ 350K";
                s.DurationText = "1–2 giờ";
                s.BadgeText = "Cứu tinh tóc xơ";
                s.ImageUrl ??= "/resources/dich_vu/phuc_hoi.jpg";
                s.HeroImageUrl = "/resources/bo_suu_tap/kieu_toc_thinh_hanh/a332180364f04545890e372ac24bc37b.jpeg";
                s.BeforeImageUrl = "/resources/before_after/75377bf482d64991a65133ce4babe449.jpg";
                s.AfterImageUrl = "/resources/before_after/906c164c51af4e7f9aad2cebc59ea6e3.jpg";
                s.PricingTableJson = """
                {
                    "Groups": [
                        {
                            "Title": "Bảng giá liệu trình phục hồi tóc hư tổn",
                            "Columns": ["Liệu trình", "Cơ bản", "Chuyên sâu Olaplex"],
                            "Items": [
                                { "Name": "Phục hồi tóc ngắn (Size S)", "Price1": "350.000đ", "Price2": "600.000đ", "Note": "Bổ sung độ ẩm & protein" },
                                { "Name": "Phục hồi tóc lửng (Size M)", "Price1": "450.000đ", "Price2": "800.000đ", "Note": "Tái tạo liên kết đứt gãy" },
                                { "Name": "Phục hồi tóc dài (Size L)", "Price1": "600.000đ", "Price2": "1.000.000đ", "Note": "Chống chẻ ngọn đứt rụng" },
                                { "Name": "Cấp cứu tóc nát do tẩy nhuộm", "Price1": "800.000đ", "Price2": "1.400.000đ", "Note": "Công nghệ hàn gắn đa tầng" }
                            ]
                        }
                    ],
                    "Addons": [
                        { "Name": "Tỉa ngọn tóc chẻ ngọn", "Price": "Miễn phí" },
                        { "Name": "Xịt dưỡng bảo vệ nhiệt tại nhà", "Price": "280.000đ" }
                    ],
                    "Note": "Hiệu quả tóc mềm mượt, dai sợi cảm nhận rõ rệt ngay từ buổi đầu tiên."
                }
                """;
                s.MethodsJson = """
                [
                    { "Title": "Hàn gắn liên kết Olaplex", "Description": "Công nghệ được cấp bằng sáng chế tái tạo các liên kết disulfide bị phá vỡ trong lõi tóc.", "Icon": "zap" },
                    { "Title": "Bổ sung Keratin sinh học", "Description": "Lấp đầy các lỗ hổng trên lớp biểu bì, giúp sợi tóc dày dặn và chống lại tác động môi trường.", "Icon": "shield" },
                    { "Title": "Hấp thủy phân Nano Ion", "Description": "Hạt sương siêu nhỏ đưa dưỡng chất thẩm thấu sâu tận tủy tóc mà không làm bết dính.", "Icon": "droplet" }
                ]
                """;
                s.StepsJson = """
                [
                    { "StepNumber": 1, "Title": "Soi và kiểm tra mức độ hư tổn", "Description": "Kiểm tra độ đàn hồi khi tóc ướt để xác định tóc thiếu ẩm, thiếu đạm hay gãy liên kết." },
                    { "StepNumber": 2, "Title": "Thanh lọc tóc & Mở biểu bì", "Description": "Gội sạch cặn hóa chất và kim loại nặng bám trên sợi tóc." },
                    { "StepNumber": 3, "Title": "Đưa tinh chất phục hồi vào lõi tóc", "Description": "Thoa đều tinh chất phục hồi nồng độ cao và massage kích hoạt dưỡng chất." },
                    { "StepNumber": 4, "Title": "Chiếu máy hấp ánh sáng sinh học", "Description": "Nhiệt độ ổn định giúp dưỡng chất liên kết chặt chẽ vào sâu bên trong sợi tóc." },
                    { "StepNumber": 5, "Title": "Khóa biểu bì & Hoàn thiện", "Description": "Xả lạnh khóa chặt dưỡng chất bên trong, sấy khô tạo kiểu nhẹ nhàng." }
                ]
                """;
                s.FaqsJson = """
                [
                    { "Question": "Sau khi phục hồi hiệu quả giữ được bao lâu?", "Answer": "Thông thường giữ từ 4–6 tuần. Để duy trì lâu dài, bạn nên kết hợp dùng kem xả hoặc ủ tóc tại nhà theo hướng dẫn của salon." },
                    { "Question": "Tóc hư tổn nặng có cần làm nhiều lần không?", "Answer": "Với tóc quá nát, liệu trình 2-3 buổi cách nhau 2 tuần sẽ giúp mái tóc hồi sinh hoàn toàn." }
                ]
                """;
                changed = true;
            }
            else
            {
                s.Slug = GenerateSlug(s.Name);
                s.Headline ??= $"{s.Name} chuyên nghiệp tại MC Hair Salon";
                s.ShortDescription ??= s.Description;
                s.PriceTagText ??= $"Từ {s.PriceFrom:N0}đ";
                s.DurationText ??= "1–2 giờ";
                changed = true;
            }

            await repository.AddServiceAsync(s, ct);
        }

        if (changed)
        {
            await db.SaveChangesAsync(ct);
        }
    }

    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "dich-vu";
        string slug = text.ToLowerInvariant().Trim();
        string[] vietnamese = ["áàảãạăắằẳẵặâấầẩẫậ", "đ", "éèẻẽẹêếềểễệ", "íìỉĩị", "óòỏõọôốồổỗộơớờởỡợ", "úùủũụưứừửữự", "ýỳỷỹỵ"];
        string[] ascii = ["a", "d", "e", "i", "o", "u", "y"];
        for (int i = 0; i < vietnamese.Length; i++)
        {
            foreach (char c in vietnamese[i])
            {
                slug = slug.Replace(c.ToString(), ascii[i]);
            }
        }
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "dich-vu" : slug;
    }
}

public sealed record PartnerDefinition(
    string Name,
    string LogoUrl,
    string Description,
    int SortOrder
);

public sealed record StylistDefinition(string Name, string ImageUrl, int SortOrder);
