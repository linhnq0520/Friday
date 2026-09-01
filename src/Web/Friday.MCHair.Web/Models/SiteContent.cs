using System.Text.RegularExpressions;

namespace Friday.MCHair.Web.Models;

public static class SiteContent
{
    public const string FacebookUrl = "https://www.facebook.com/profile.php?id=61551835762411";
    public const string YouTubeUrl = "https://www.youtube.com/@mchairsalon";

    public const string DefaultHotline = "0988305371";
    public const string DefaultOpeningHours = "08:30 – 20:00";

    public const string DefaultAddress =
        "14D Cống Quỳnh, Phường Cầu Ông Lãnh, TP. Hồ Chí Minh, Việt Nam";

    public const string DefaultAddressShort = "14D Cống Quỳnh, P. Cầu Ông Lãnh, TP.HCM";

    public const string DefaultMapsUrl =
        "https://www.google.com/maps/search/?api=1&query=14D+C%E1%BB%91ng+Qu%E1%BB%B3nh,+Ph%C6%B0%E1%BB%9Dng+C%E1%BA%A7u+%C3%94ng+L%C3%A3nh,+TP.+H%E1%BB%93+Ch%C3%AD+Minh";

    public const string DefaultZaloPhone = "0988305371";

    public const string DefaultMessengerUrl = "https://m.me/61551835762411";

    public const string DefaultBookingEasySalonUrl = "https://booking.easysalon.vn/mchairsalon";

    public const string DefaultBookingMode = BookingSettings.ModeEasySalon;

    public const string IconHotlinePath = "/images/icon-hotline.webp";

    public const string IconZaloPath = "/images/icon-zalo.webp";

    public const string IconMessengerPath = "/images/icon-messenger.svg";

    public const string LogoPath = "/resources/logo.jpg";

    public const string FaviconIcoPath = "/favicon.ico";
    public const string Favicon16Path = "/favicon-16x16.png";
    public const string Favicon32Path = "/favicon-32x32.png";
    public const string AppleTouchIconPath = "/apple-touch-icon.png";

    public static readonly string[] HeroSlides =
    [
        "/resources/khong_gian/khong-gian1.jpg",
        "/resources/khong_gian/khong-gian14.jpg",
        "/resources/khong_gian/khong-gian20.jpg",
        "/resources/khong_gian/khong-gian6.jpg",
        "/resources/khong_gian/khong-gian32.jpg",
    ];

    public static readonly string[] SpaceShowcase =
    [
        "/resources/khong_gian/khong-gian7.jpg",
        "/resources/khong_gian/khong-gian17.jpg",
        "/resources/khong_gian/khong-gian22.jpg",
        "/resources/khong_gian/khong-gian28.jpg",
        "/resources/khong_gian/khong-gian30.jpg",
    ];

    public const string AboutStoryTitle = "Về MC Hair";

    public const string AboutTagline =
        "MC Hair – Opening a New Door to Beauty, Confidence and New Opportunities.";

    public const string AboutStoryBody = """
        MC Hair được tạo ra với mong muốn mở ra cánh cửa mới cho vẻ đẹp và sự tự tin của mỗi khách hàng. Chúng tôi tin rằng một mái tóc đẹp không chỉ là xu hướng thời trang mà còn là cách để mỗi người thể hiện cá tính, phong cách và phiên bản tốt hơn của chính mình.

        Tên gọi MC Hair mang trong mình những giá trị cốt lõi mà chúng tôi luôn theo đuổi.

        “M – Mode” đại diện cho thời trang và sự đổi mới, thể hiện tinh thần luôn cập nhật những xu hướng tóc hiện đại, tinh tế và phù hợp với từng khách hàng.

        “C – Mirror Confidence” là sự tự tin khi nhìn vào chính mình trong gương, bởi chúng tôi tin rằng một diện mạo mới có thể mang đến nguồn năng lượng tích cực và sự tự tin trong cuộc sống.

        Bên cạnh đó, “M – Masterpiece Creation” còn mang ý nghĩa tạo nên những tác phẩm nghệ thuật trên mái tóc bằng kỹ thuật, sự tỉ mỉ và niềm đam mê của đội ngũ hairstylist tại MC Hair. Mỗi kiểu tóc không chỉ đơn thuần là làm đẹp mà còn là dấu ấn riêng dành cho từng khách hàng.

        MC Hair cũng theo đuổi hai giá trị quan trọng là “Mộc – Chân” và “Mỹ – Chất”. “Mộc – Chân” thể hiện sự tự nhiên và chân thành trong từng trải nghiệm, từ cách lắng nghe, tư vấn đến việc lựa chọn giải pháp phù hợp nhất cho mái tóc. “Mỹ – Chất” là vẻ đẹp đi cùng chất lượng bền vững, nơi mái tóc không chỉ đẹp bên ngoài mà còn được chăm sóc khỏe mạnh từ bên trong.

        Không đơn thuần là một salon tóc, MC Hair mong muốn trở thành nơi đồng hành cùng khách hàng trên hành trình thay đổi bản thân, khám phá vẻ đẹp riêng và tạo nên những cơ hội mới trong cuộc sống.
        """;

    public const string MissionTitle = "Sứ mệnh";

    public const string MissionBody = """
        MC Hair ra đời với sứ mệnh không chỉ tạo nên những kiểu tóc đẹp, mà còn đánh thức sự tự tin và thần thái riêng trong mỗi khách hàng.
        Chúng tôi tin rằng, mỗi mái tóc là một “tuyên ngôn cá tính”, và mỗi lần thay đổi là một bước tiến gần hơn đến phiên bản tốt nhất của chính mình.

        MC Hair hướng đến việc mang lại trải nghiệm làm đẹp hiện đại, tinh tế và phù hợp với từng khuôn mặt, phong cách sống.
        Không chạy theo xu hướng một cách đại trà, chúng tôi cá nhân hoá từng dịch vụ – để mỗi khách hàng khi bước ra đều cảm thấy “đúng với mình” nhất.

        Với MC Hair, làm tóc không chỉ là dịch vụ – mà là hành trình nâng tầm diện mạo, khơi dậy sự tự tin và giúp bạn tỏa sáng theo cách riêng.
        """;

    public const string VisionTitle = "Tầm nhìn";

    public const string VisionBody = """
        MC Hair hướng đến trở thành thương hiệu salon được tin chọn hàng đầu trong phân khúc làm đẹp hiện đại, nơi khách hàng không chỉ đến để thay đổi kiểu tóc mà còn tìm thấy sự tự tin và phong cách riêng.
        Chúng tôi mong muốn xây dựng một không gian làm đẹp chuyên nghiệp, nơi mỗi trải nghiệm đều chỉn chu – từ kỹ thuật, dịch vụ đến cảm xúc khách hàng.

        Trong tương lai, MC Hair không chỉ là một salon, mà còn là điểm đến truyền cảm hứng về cái đẹp, xu hướng và sự tự tin cho thế hệ trẻ.
        """;

    public static readonly CoreValueItem[] CoreValues =
    [
        new(
            "Chất lượng đặt lên hàng đầu",
            "Luôn đảm bảo kỹ thuật, sản phẩm và dịch vụ đạt tiêu chuẩn cao nhất."
        ),
        new(
            "Cá nhân hoá trải nghiệm",
            "Mỗi khách hàng là một phong cách riêng – MC Hair luôn tư vấn và thiết kế phù hợp nhất."
        ),
        new(
            "Tận tâm & chuyên nghiệp",
            "Phục vụ bằng sự chân thành, thái độ chuyên nghiệp và tinh thần trách nhiệm."
        ),
        new(
            "Cập nhật xu hướng liên tục",
            "Luôn học hỏi, sáng tạo để mang đến những kiểu tóc hiện đại, dẫn đầu xu hướng."
        ),
        new(
            "Xây dựng sự tự tin cho khách hàng",
            "Không chỉ làm đẹp bên ngoài, MC Hair giúp khách hàng cảm thấy tự tin từ bên trong."
        ),
        new(
            "Phát triển bền vững",
            "Tạo dựng uy tín lâu dài dựa trên chất lượng thật và sự hài lòng của khách hàng."
        ),
    ];

    public static string[] SplitParagraphs(string body) =>
        Regex
            .Split(body.Trim(), @"\r?\n\s*\r?\n")
            .Select(p => p.Trim())
            .Where(p => p.Length > 0)
            .ToArray();
}

public sealed record CoreValueItem(string Title, string Description);
