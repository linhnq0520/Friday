namespace Friday.MCHair.Web.Models;

public static class SiteContent
{
    public const string FacebookUrl = "https://www.facebook.com/profile.php?id=61551835762411";

    public const string DefaultHotline = "0988305371";
    public const string DefaultOpeningHours = "08:30 – 20:00";

    public const string DefaultAddress =
        "14D Cống Quỳnh, Phường Cầu Ông Lãnh, TP. Hồ Chí Minh, Việt Nam";

    public const string DefaultAddressShort = "14D Cống Quỳnh, P. Cầu Ông Lãnh, TP.HCM";

    public const string DefaultMapsUrl =
        "https://www.google.com/maps/search/?api=1&query=14D+C%E1%BB%91ng+Qu%E1%BB%B3nh,+Ph%C6%B0%E1%BB%9Dng+C%E1%BA%A7u+%C3%94ng+L%C3%A3nh,+TP.+H%E1%BB%93+Ch%C3%AD+Minh";

    public const string DefaultZaloPhone = "0988305371";

    public const string DefaultMessengerUrl = "https://m.me/61551835762411";

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
}

public sealed record CoreValueItem(string Title, string Description);
