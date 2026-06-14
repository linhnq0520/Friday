namespace Friday.MCHair.Web.Models;

public sealed record SectionEnglishContent(string? Title, string? Subtitle, string? Body);

public static class SectionContentEn
{
    private static readonly Dictionary<string, SectionEnglishContent> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["hero"] = new(
                "MC Hair Salon",
                "A statement of personality through hair",
                "A modern, refined and personalized beauty experience — where you find confidence and your own style."
            ),
            ["about"] = new(
                "About MC Hair",
                null,
                "MC Hair was born with a mission not only to create beautiful hairstyles, but to awaken confidence and personal charisma in every client."
            ),
            ["gallery_intro"] = new(
                "Hot hairstyles",
                "2026 trend collection",
                "Discover trending hairstyles and colors that help you shine."
            ),
            ["services_intro"] = new(
                "Hair services",
                "Transparent pricing — quality assured",
                "Popular salon services at competitive prices."
            ),
            ["partners_intro"] = new(
                "Partners",
                null,
                "We work with major, trusted partners including premium product brands used in our salon services."
            ),
            ["feedback_intro"] = new(
                "Client feedback",
                null,
                "What our clients share about their experience at MC Hair Salon."
            ),
        };

    public static SectionEnglishContent? TryGet(string sectionKey) =>
        Map.TryGetValue(sectionKey, out SectionEnglishContent? content) ? content : null;
}
