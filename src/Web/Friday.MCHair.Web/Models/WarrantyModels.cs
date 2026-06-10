namespace Friday.MCHair.Web.Models;

public sealed class WarrantyPageData
{
    public string Title { get; set; } = "Chế độ bảo hành";

    public string Lead { get; set; } =
        "Cam kết chất lượng dịch vụ và sự hài lòng của khách hàng sau mỗi lần làm tóc.";

    public string MetaDescription { get; set; } =
        "Chính sách bảo hành dịch vụ làm tóc tại MC Hair Salon – cam kết chất lượng và hài lòng khách hàng.";

    public List<WarrantySectionData> Sections { get; set; } = [];
}

public sealed class WarrantySectionData
{
    public string Title { get; set; } = string.Empty;

  /// <summary>paragraph | list</summary>
    public string Format { get; set; } = "paragraph";

    public string Body { get; set; } = string.Empty;
}

public static class WarrantyDefaults
{
    public const string WarrantyPeriodsBody =
        "<strong>Nhuộm:</strong> Bảo hành 30 ngày.\n<strong>Uốn / duỗi:</strong> Bảo hành 60 ngày.\n<strong>Phục hồi:</strong> Bảo hành 30 ngày.\n<strong>Cắt tóc:</strong> Bảo hành 15 ngày.";

    public static WarrantyPageData Create() =>
        new()
        {
            Title = "Chế độ bảo hành",
            Lead =
                "Cam kết chất lượng dịch vụ và sự hài lòng của khách hàng sau mỗi lần làm tóc.",
            MetaDescription =
                "Chính sách bảo hành dịch vụ làm tóc tại MC Hair Salon – cam kết chất lượng và hài lòng khách hàng.",
            Sections =
            [
                new()
                {
                    Title = "1. Phạm vi bảo hành",
                    Format = "paragraph",
                    Body =
                        "MC Hair áp dụng chế độ bảo hành cho các dịch vụ nhuộm, uốn, duỗi và phục hồi tóc trong thời gian quy định tùy theo từng loại dịch vụ. Bảo hành áp dụng khi sản phẩm và kỹ thuật thực hiện đúng quy trình chuẩn của salon.",
                },
                new()
                {
                    Title = "2. Thời gian bảo hành",
                    Format = "list",
                    Body = WarrantyPeriodsBody,
                },
                new()
                {
                    Title = "3. Điều kiện áp dụng",
                    Format = "list",
                    Body =
                        "Khách hàng giữ hóa đơn hoặc xác nhận đặt lịch tại MC Hair.\nTóc được chăm sóc theo hướng dẫn sau dịch vụ của stylist.\nKhông tự ý dùng hóa chất, nhuộm hoặc xử lý nhiệt quá mức tại nơi khác trong thời gian bảo hành.\nLiên hệ salon trước khi đến để được sắp xếp thời gian chỉnh sửa.",
                },
                new()
                {
                    Title = "4. Trường hợp không áp dụng",
                    Format = "list",
                    Body =
                        "Tóc hư tổn nặng do tự chăm sóc sai cách sau dịch vụ.\nYêu cầu thay đổi kiểu/màu hoàn toàn khác so với dịch vụ ban đầu.\nQuá thời hạn bảo hành đã thông báo khi làm dịch vụ.",
                },
                new()
                {
                    Title = "5. Liên hệ hỗ trợ",
                    Format = "paragraph",
                    Body =
                        "Mọi thắc mắc về bảo hành, vui lòng liên hệ hotline <strong id=\"warranty-hotline\">0988305371</strong> hoặc nhắn tin qua <a href=\"https://www.facebook.com/profile.php?id=61551835762411\" target=\"_blank\" rel=\"noopener\">fanpage MC Hair</a>.",
                },
            ],
        };
}
