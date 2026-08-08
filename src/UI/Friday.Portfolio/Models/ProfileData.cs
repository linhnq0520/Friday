namespace Friday.Portfolio.Models;

/// <summary>
/// Static profile / CV content for the personal site.
/// Update here when the resume changes.
/// </summary>
public static class ProfileData
{
    public const string FullName = "Nguyen Quoc Linh";
    public const string Nickname = "Thomas";
    public const string Title = "Backend Engineer";
    public const string Tagline = "Fintech & core banking · distributed systems · ASP.NET Core";
    public const string Mission =
        "I write field notes from shipping payment and core-banking systems — " +
        "the modernization paths, messaging trade-offs, and Azure deploy habits that hold up under load.";
    public const string Phone = "+84 943 894 177";
    public const string Email = "zquoclinh@gmail.com";
    public const string LinkedIn = "https://linkedin.com/in/quoclinh0520";
    public const string LinkedInDisplay = "linkedin.com/in/quoclinh0520";
    public const string NuGet = "https://www.nuget.org/packages/LinKit.Core";
    public const string BonGitHub = "https://github.com/linhnq0520/Bon";
    public const string Location = "Ho Chi Minh City, Vietnam";
    public const string WritingSince = "2026";

    public const string Summary =
        "Backend Engineer with 4 years of experience in fintech and core banking systems, " +
        "specializing in distributed architectures and microservices-based solutions. " +
        "Proven track record in modernizing legacy .NET Framework applications to ASP.NET Core, " +
        "improving system scalability, performance, and maintainability.";

    public const string Objective =
        "Career objective: transition into a Technical Lead role — leverage enterprise system design " +
        "while mentoring teams to deliver scalable, high-performance solutions. " +
        "Open to onsite work with international clients.";

    public const string AuthorNote =
        "I started this site as a learning lab: publish what I actually ship in fintech/.NET, " +
        "keep a public trail of deep-dives, and turn recurring topics into structured courses. " +
        "No fluff — just notes I wish I had during WCF migrations, OTP APIs, and Azure CI/CD.";

    public static readonly IReadOnlyList<string> Languages =
        ["C#", "SQL", "TypeScript", "Java", "JavaScript"];

    public static readonly IReadOnlyList<string> Frameworks =
        ["ASP.NET Core", "Entity Framework", "React Native", "React"];

    public static readonly IReadOnlyList<string> Databases =
        ["MS SQL Server", "Oracle", "PostgreSQL"];

    public static readonly IReadOnlyList<string> Tools =
        ["RabbitMQ", "gRPC", "Redis", "Docker", "YARP", "GitHub/GitLab", "IIS", "JMeter"];

    public static readonly IReadOnlyList<string> Topics =
        [".NET 10", "ASP.NET Core", "gRPC", "RabbitMQ", "Azure", "System Design", "Blazor"];

    public static readonly IReadOnlyList<ExperienceItem> Experiences =
    [
        new(
            "VietUnion Online Services Corporation (Payoo)",
            ".NET Developer",
            "Apr 2025 – Present",
            "Ho Chi Minh City",
            [
                "Delivered backlog features for core payment services (QRCode, POS, mPOS, Pay4Biz).",
                "Refactored and enhanced legacy WCF (.NET Framework) services.",
                "Migrated backend services to ASP.NET Core following modern RESTful architecture.",
                "Optimized QR code generation API under high-concurrency load testing (JMeter), reducing average response time from ~30s to ~5s.",
                "Conducted code reviews and enforced coding standards."
            ],
            ["C#", ".NET Framework", "ASP.NET Core"]),
        new(
            "JUST-IN-TIME Solutions",
            "Software Developer",
            "Oct 2022 – Apr 2025",
            "Vietnam / Myanmar (onsite)",
            [
                "Onsite technical support in Myanmar for deployment and production issue resolution.",
                "Designed secure OTP authentication and teller transaction APIs for core banking.",
                "Implemented JWT-based authentication and request validation.",
                "Integrated distributed services via gRPC and RabbitMQ on Neptune Core Banking.",
                "Maintained Optimal9 legacy core banking (Java, Oracle, VB.NET) and resolved production issues.",
                "Onboarded new team members and supported operational stability."
            ],
            ["ASP.NET Core", "React Native", "SQL Server", "RabbitMQ", "Redis", "gRPC", "Docker"]),
        new(
            "Freelance — Microservice Orchestration Platform",
            "System Architect",
            "2025 – Present",
            "Remote",
            [
                "Architected orchestration flow for distributed services (routing, workflow, communication).",
                "Designed synchronous (gRPC) and asynchronous (RabbitMQ) communication patterns.",
                "Built API gateway layer using YARP Reverse Proxy.",
                "Implemented compile-time optimizations using C# Source Generators."
            ],
            ["ASP.NET Core", "gRPC", "RabbitMQ", "YARP", "Source Generators", "Docker"])
    ];

    public static readonly IReadOnlyList<ProjectItem> Projects =
    [
        new(
            "LinKit.Core",
            "Lightweight utility and core abstraction library for .NET. Published as a public NuGet package.",
            ["C#", ".NET Standard", "NuGet", "LINQ", "Expression Trees"],
            NuGet,
            "NuGet"),
        new(
            "Bon Framework",
            "Microservices framework built with .NET — EF Core data access, DI service resolution, and RabbitMQ messaging.",
            ["ASP.NET Core", "EF Core", "RabbitMQ"],
            BonGitHub,
            "GitHub")
    ];

    public static readonly IReadOnlyList<EducationItem> Education =
    [
        new("University of Science — VNUHCM", "Java programming certification", "5/2022 – 11/2022"),
        new("University of Economics Ho Chi Minh City", "Bachelor of Commercial Business", "7/2017 – 5/2021")
    ];

    public static readonly IReadOnlyList<AudienceItem> Audiences =
    [
        new("01", "First-job .NET devs", "Clear notes on APIs, EF Core habits, and how banking-style systems are structured."),
        new("02", "Mid-level engineers", "Modernization paths, messaging trade-offs, and load-test lessons from payment services."),
        new("03", "Tech leads / architects", "Gateway, gRPC, and orchestration decisions used on real distributed platforms."),
        new("04", "Career switchers", "A practical map of modern .NET without decade-old Stack Overflow detours.")
    ];
}
