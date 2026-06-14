namespace Friday.MCHair.Web.Models;

public static class PriceListDefaultsEn
{
    public static PriceListData Create() =>
        new()
        {
            PriceUnitNote =
                "Unit: thousand VND. Prices may vary based on actual hair condition.",
            LengthGuide =
            [
                new HairLengthGuideItem { Size = "S", Description = "Short hair, chin length or above" },
                new HairLengthGuideItem { Size = "M", Description = "Chin to shoulders" },
                new HairLengthGuideItem { Size = "L", Description = "Shoulders to chest" },
                new HairLengthGuideItem { Size = "XL", Description = "Below chest" },
            ],
            Groups =
            [
                Group(
                    0,
                    0,
                    "Haircut",
                    [
                        Item("Master hair artist", "350"),
                        Item("Hair artist", "250"),
                        Item("Bang trim", "50"),
                        Item("Men's cut", "100"),
                        Item("Trim (no wash)", "100"),
                    ]
                ),
                Group(
                    0,
                    1,
                    "Perm & straightening",
                    [
                        Item(
                            "Straightening",
                            "Roots 600–1,000 · S 1,000 · M 1,200 · L 1,400 · XL 1,600"
                        ),
                        Item("Perm", "S 1,000 · M 1,200 · L 1,400 · XL 1,600"),
                        Item("Keratin", "S 1,500 · M 1,800 · L 2,000 · XL 2,200"),
                        Item("Bang perm", "300"),
                        Item("Volume perm", "500"),
                        Item("Volume removal", "350"),
                        Item("Root perm (extra)", "300"),
                        Item("Men's perm", "500 – 1,000"),
                        Item("XXL size extra", "200"),
                    ]
                ),
                Group(
                    0,
                    2,
                    "Other",
                    [
                        Item("Hair treatment", "300 – 500"),
                        Item("Hair wash", "100"),
                        Item("Extension wash", "150"),
                        Item("Styling", "100"),
                    ]
                ),
                Group(
                    1,
                    0,
                    "Color & bleach",
                    [
                        Item("Color", "Roots 500–1,000 · S 800 · M 1,000 · L 1,200 · XL 1,400"),
                        Item("Lift", "S 600 · M 700 · L 800 · XL 900"),
                        Item("Keratin color", "S 1,300 · M 1,500 · L 1,800 · XL 2,100"),
                        Item("Bleach", "S 1,000 · M 1,200 · L 1,400 · XL 1,600"),
                        Item("Extension bleach", "1,000 – 1,500 / session"),
                        Item("Color removal / cover", "S 800 · M 900 · L 1,000 · XL 1,200"),
                    ]
                ),
                Group(
                    1,
                    1,
                    "Creative color",
                    [
                        Item("Balayage / Ombre / Airtouch", "M 3,000 · L 3,500 · XL 4,000"),
                        Item("Hidden", "S 1,000 · M 1,200 · L 1,400 · XL 1,600"),
                        Item("Highlight", "S 1,200 · M 1,400 · L 1,600 · XL 1,800"),
                    ]
                ),
                Group(
                    2,
                    0,
                    "Hair extensions",
                    [
                        Item("40 cm", "25 / strand"),
                        Item("50 cm", "28 / strand"),
                        Item("60 cm", "32 / strand"),
                        Item("Light extensions", "80 / weft"),
                        Item("Extension lift", "10 / weft"),
                        Item("Extension removal", "4 / weft"),
                        Item("Extension application", "6 / weft"),
                    ]
                ),
                Group(
                    2,
                    1,
                    "Protection & damage reduction",
                    [
                        Item("Olaplex", "S 600 · M 800 · L 1,000 · XL 1,200"),
                        Item("ATS", "S 1,000 · M 1,200 · L 1,400 · XL 1,600"),
                    ]
                ),
                Group(
                    2,
                    2,
                    "Care & nourishment",
                    [
                        Item("Milbon", "S 800 · M 1,000 · L 1,200 · XL 1,400"),
                        Item("Number 003", "S 1,000 · M 1,200 · L 1,400 · XL 1,600"),
                    ]
                ),
                Group(
                    2,
                    3,
                    "Repair & treatment",
                    [
                        Item("Keratin", "S 1,200 · M 1,400 · L 1,600 · XL 1,800"),
                        Item("Kerathphy", "S 1,800 · M 2,200 · L 2,600 · XL 3,000"),
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
