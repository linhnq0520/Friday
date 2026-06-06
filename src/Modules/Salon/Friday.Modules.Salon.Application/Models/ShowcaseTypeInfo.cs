using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Application.Models;

public static class ShowcaseTypeInfo
{
    public static readonly ShowcaseType[] AllTypes = [ShowcaseType.Feedback, ShowcaseType.BeforeAfter];

    public static string GetFolderSlug(ShowcaseType type) =>
        type switch
        {
            ShowcaseType.Feedback => "feedback",
            ShowcaseType.BeforeAfter => "before_after",
            _ => type.ToString().ToLowerInvariant(),
        };

}
