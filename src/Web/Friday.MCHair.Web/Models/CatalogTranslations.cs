namespace Friday.MCHair.Web.Models;

public static class CatalogTranslations
{
    private static readonly Dictionary<string, (string Name, string Description)> Services =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Cắt tóc"] = (
                "Haircut",
                "Master / Hair artist — face-shape consultation"
            ),
            ["Uốn / Duỗi"] = (
                "Perm / Straightening",
                "Perm, straightening, keratin — priced by hair length"
            ),
            ["Nhuộm / Tẩy"] = (
                "Color / Bleach",
                "Color, bleach, lift — professional products"
            ),
            ["Nhuộm thiết kế"] = (
                "Creative color",
                "Balayage, highlight, hidden color and more"
            ),
            ["Nối tóc"] = (
                "Hair extensions",
                "Various lengths and methods — consultation required"
            ),
            ["Phục hồi / Olaplex"] = (
                "Repair / Olaplex",
                "Deep repair and bond-building treatments"
            ),
            ["Gội / Tạo kiểu"] = (
                "Wash / Styling",
                "Wash, blow-dry and styling services"
            ),
        };

    private static readonly Dictionary<string, string> PartnerDescriptions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["OLAPLEX"] =
                "Olaplex is one of the world's largest hair care brands with over 100 patents.",
            ["MOROCCANOIL"] =
                "Moroccanoil is a globally renowned hair care brand recommended by professional stylists.",
            ["B3 BRAZILIAN"] =
                "B3 Brazilian Bond Builder — a leading US brand chosen by professional salons for severe damage and demanding color clients.",
            ["L'Oréal"] =
                "L'Oréal Paris is a world-leading beauty brand making luxury beauty accessible to everyone.",
        };

    public static string ServiceName(string name, bool english)
    {
        if (english && Services.TryGetValue(name, out (string Name, string Description) pair))
        {
            return pair.Name;
        }

        return name;
    }

    public static string ServiceDescription(string name, string? description, bool english)
    {
        if (english && Services.TryGetValue(name, out (string Name, string Description) pair))
        {
            return pair.Description;
        }

        return description ?? string.Empty;
    }

    public static string PartnerDescription(string name, string? description, bool english) =>
        english && PartnerDescriptions.TryGetValue(name, out string? en)
            ? en
            : description ?? string.Empty;
}
