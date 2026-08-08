namespace Friday.Portfolio.Models;

public sealed record ExperienceItem(
    string Company,
    string Role,
    string Period,
    string Location,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> Tech);

public sealed record ProjectItem(
    string Name,
    string Description,
    IReadOnlyList<string> Tech,
    string Url,
    string UrlLabel);

public sealed record EducationItem(
    string School,
    string Degree,
    string Period);

public sealed record BlogArticle(
    string Slug,
    string Title,
    string Summary,
    DateOnly Published,
    int MinutesToRead,
    IReadOnlyList<string> Tags,
    string MarkdownPath,
    bool Featured = false);

public sealed record Course(
    string Slug,
    string Title,
    string Summary,
    string Level,
    int LessonCount,
    bool IsFree,
    IReadOnlyList<string> Topics,
    IReadOnlyList<string> Outcomes,
    string Status = "In progress");

public sealed record AudienceItem(
    string Number,
    string Title,
    string Description);
