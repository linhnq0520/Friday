namespace Friday.MCHair.Web.Models;

public sealed class PriceListData
{
    public string PriceUnitNote { get; set; } =
        "Đơn vị: nghìn đồng. Giá có thể điều chỉnh theo tình trạng tóc thực tế.";

    public List<HairLengthGuideItem> LengthGuide { get; set; } = [];

    public List<PriceGroupData> Groups { get; set; } = [];

    public IReadOnlyList<PriceColumnView> ToColumns()
    {
        int maxColumn = Groups.Count == 0 ? 0 : Groups.Max(g => g.ColumnIndex);
        return Enumerable
            .Range(0, maxColumn + 1)
            .Select(columnIndex =>
            {
                List<PriceGroupData> groups = Groups
                    .Where(g => g.ColumnIndex == columnIndex)
                    .OrderBy(g => g.SortOrder)
                    .ToList();
                return new PriceColumnView(groups);
            })
            .ToList();
    }
}

public sealed class HairLengthGuideItem
{
    public string Size { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class PriceGroupData
{
    public int ColumnIndex { get; set; }
    public int SortOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<PriceItemData> Items { get; set; } = [];
}

public sealed class PriceItemData
{
    public string Name { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
}

public sealed record PriceColumnView(IReadOnlyList<PriceGroupData> Groups);

public static class PriceListDefaults
{
    public static PriceListData Create() =>
        new()
        {
            PriceUnitNote =
                "Đơn vị: nghìn đồng. Giá có thể điều chỉnh theo tình trạng tóc thực tế.",
            LengthGuide =
            [
                new HairLengthGuideItem { Size = "S", Description = "Tóc ngắn, từ cằm trở lên" },
                new HairLengthGuideItem { Size = "M", Description = "Từ cằm đến vai" },
                new HairLengthGuideItem { Size = "L", Description = "Từ vai đến ngực" },
                new HairLengthGuideItem { Size = "XL", Description = "Dài qua ngực" },
            ],
            Groups =
            [
                Group(
                    0,
                    0,
                    "Cắt tóc",
                    [
                        Item("Master hair artist", "350"),
                        Item("Hair artist", "250"),
                        Item("Cắt mái", "50"),
                        Item("Cắt nam", "100"),
                        Item("Tỉa tóc (không gội)", "100"),
                    ]
                ),
                Group(
                    0,
                    1,
                    "Uốn & duỗi",
                    [
                        Item(
                            "Duỗi tóc",
                            "Chân 600 – 1.000 · S 1.000 · M 1.200 · L 1.400 · XL 1.600"
                        ),
                        Item("Uốn tóc", "S 1.000 · M 1.200 · L 1.400 · XL 1.600"),
                        Item("Thuần chay", "S 1.500 · M 1.800 · L 2.000 · XL 2.200"),
                        Item("Uốn mái", "300"),
                        Item("Uốn phồng", "500"),
                        Item("Xả phồng", "350"),
                        Item("Uốn sát chân (phụ phí)", "300"),
                        Item("Uốn nam", "500 – 1.000"),
                        Item("Phụ phí size XXL", "200"),
                    ]
                ),
                Group(
                    0,
                    2,
                    "Khác",
                    [
                        Item("Hấp tóc", "300 – 500"),
                        Item("Gội đầu", "100"),
                        Item("Gội tóc nối", "150"),
                        Item("Tạo kiểu", "100"),
                    ]
                ),
                Group(
                    1,
                    0,
                    "Nhuộm & tẩy",
                    [
                        Item("Nhuộm", "Chân 500 – 1.000 · S 800 · M 1.000 · L 1.200 · XL 1.400"),
                        Item("Nâng sáng", "S 600 · M 700 · L 800 · XL 900"),
                        Item("Thuần chay", "S 1.300 · M 1.500 · L 1.800 · XL 2.100"),
                        Item("Tẩy tóc", "S 1.000 · M 1.200 · L 1.400 · XL 1.600"),
                        Item("Tẩy nối chân", "1.000 – 1.500 / lần"),
                        Item("Bóc màu / phủ màu", "S 800 · M 900 · L 1.000 · XL 1.200"),
                    ]
                ),
                Group(
                    1,
                    1,
                    "Nhuộm thiết kế",
                    [
                        Item("Balayage / Ombre / Airtouch", "M 3.000 · L 3.500 · XL 4.000"),
                        Item("Hidden", "S 1.000 · M 1.200 · L 1.400 · XL 1.600"),
                        Item("Highlight", "S 1.200 · M 1.400 · L 1.600 · XL 1.800"),
                    ]
                ),
                Group(
                    2,
                    0,
                    "Nối tóc",
                    [
                        Item("40 cm", "25 / sợi"),
                        Item("50 cm", "28 / sợi"),
                        Item("60 cm", "32 / sợi"),
                        Item("Nối light", "80 / tép"),
                        Item("Nâng nối", "10 / tép"),
                        Item("Tháo nối", "4 / tép"),
                        Item("Công nối", "6 / tép"),
                    ]
                ),
                Group(
                    2,
                    1,
                    "Bảo vệ & giảm hư tổn",
                    [
                        Item("Olaplex", "S 600 · M 800 · L 1.000 · XL 1.200"),
                        Item("ATS", "S 1.000 · M 1.200 · L 1.400 · XL 1.600"),
                    ]
                ),
                Group(
                    2,
                    2,
                    "Chăm sóc & dưỡng tóc",
                    [
                        Item("Milbon", "S 800 · M 1.000 · L 1.200 · XL 1.400"),
                        Item("Number 003", "S 1.000 · M 1.200 · L 1.400 · XL 1.600"),
                    ]
                ),
                Group(
                    2,
                    3,
                    "Phục hồi & chữa trị",
                    [
                        Item("Keratin", "S 1.200 · M 1.400 · L 1.600 · XL 1.800"),
                        Item("Kerathphy", "S 1.800 · M 2.200 · L 2.600 · XL 3.000"),
                    ]
                ),
            ],
        };

    private static PriceGroupData Group(
        int columnIndex,
        int sortOrder,
        string title,
        List<PriceItemData> items
    ) =>
        new()
        {
            ColumnIndex = columnIndex,
            SortOrder = sortOrder,
            Title = title,
            Items = items,
        };

    private static PriceItemData Item(string name, string price) =>
        new() { Name = name, Price = price };
}
