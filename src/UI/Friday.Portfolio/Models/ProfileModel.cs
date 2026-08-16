namespace Friday.Portfolio.Models;

public class ProfileModel
{
    public string FullName { get; set; } = "Nguyen Quoc Linh";
    public string Nickname { get; set; } = "Thomas";
    public string Title { get; set; } = "Software Developer";
    public string Tagline { get; set; } = "Fintech & core banking · distributed systems · ASP.NET Core";
    public string Mission { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = "20/05/1999";
    public string Phone { get; set; } = "+84 943 894 177";
    public string Email { get; set; } = "zquoclinh@gmail.com";
    public string LinkedIn { get; set; } = "https://linkedin.com/in/quoclinh0520";
    public string LinkedInDisplay { get; set; } = "linkedin.com/in/quoclinh0520";
    public string WebsiteUrl { get; set; } = "";
    public string NuGet { get; set; } = "https://www.nuget.org/packages/LinKit.Core";
    public string BonGitHub { get; set; } = "https://github.com/linhnq0520/Bon";
    public string Location { get; set; } = "Ho Chi Minh City, Vietnam";
    public string WritingSince { get; set; } = "2026";
    public string Summary { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string AuthorNote { get; set; } = string.Empty;

    public List<string> Languages { get; set; } = [];
    public List<string> Frameworks { get; set; } = [];
    public List<string> Databases { get; set; } = [];
    public List<string> Tools { get; set; } = [];

    public List<ExperienceModel> Experiences { get; set; } = [];
    public List<ProjectModel> Projects { get; set; } = [];
    public List<EducationModel> Education { get; set; } = [];
}

public class ExperienceModel
{
    public string Company { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> Bullets { get; set; } = [];
    public List<string> Tags { get; set; } = [];
}

public class ProjectModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> TechStack { get; set; } = [];
    public string Url { get; set; } = string.Empty;
    public string UrlLabel { get; set; } = string.Empty;
}

public class EducationModel
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
}
