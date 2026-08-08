namespace Friday.Portfolio.Models;

/// <summary>
/// Catalog for blog posts and courses (metadata). Bodies live as Markdown under wwwroot/content.
/// Sample entries are for learning/demo — unfinished courses stay Status = "In progress".
/// </summary>
public static class SiteContent
{
    public static readonly IReadOnlyList<BlogArticle> Posts =
    [
        new(
            "wcf-to-aspnet-core-migration",
            "Migrating WCF payment services to ASP.NET Core",
            "A practical checklist from legacy .NET Framework/WCF endpoints to ASP.NET Core REST — contracts, auth, and cutover without breaking POS clients.",
            new DateOnly(2026, 8, 1),
            12,
            ["dotnet", "aspnetcore", "migration"],
            "content/blog/wcf-to-aspnet-core-migration.md",
            Featured: true),
        new(
            "grpc-vs-rabbitmq-banking",
            "gRPC vs RabbitMQ in core banking flows",
            "When teller/OTP paths need sync RPC and when ledger-style work should go async — lessons from Neptune-style architectures.",
            new DateOnly(2026, 7, 20),
            15,
            ["architecture", "grpc", "rabbitmq"],
            "content/blog/grpc-vs-rabbitmq-banking.md",
            Featured: true),
        new(
            "blazor-wasm-azure-swa-cicd",
            "Blazor WASM on Azure Static Web Apps with GitHub Actions",
            "Publish a personal .NET site for free: project layout under src/UI, deployment token, and a CI pipeline that survives PR previews.",
            new DateOnly(2026, 8, 8),
            10,
            ["blazor", "azure", "cicd"],
            "content/blog/blazor-wasm-azure-swa-cicd.md",
            Featured: true),
        new(
            "qr-api-jmeter-optimization",
            "Cutting QR API latency from 30s to 5s under load",
            "What JMeter revealed about a high-concurrency QR generation endpoint — and the fixes that actually moved p95.",
            new DateOnly(2026, 6, 15),
            11,
            ["performance", "dotnet", "jmeter"],
            "content/blog/qr-api-jmeter-optimization.md")
    ];

    public static readonly IReadOnlyList<Course> Courses =
    [
        new(
            "aspnet-core-backend-fundamentals",
            "ASP.NET Core Backend Fundamentals",
            "Build production-shaped APIs: routing, validation, auth, EF Core habits, and clean layering — the baseline for fintech backends.",
            "Beginner → Intermediate",
            24,
            IsFree: true,
            Topics: ["ASP.NET Core", "EF Core", "JWT", "Clean Architecture"],
            Outcomes:
            [
                "Scaffold a modular API you can extend",
                "Secure endpoints with JWT and validation",
                "Apply repository/unit-of-work patterns without over-engineering"
            ],
            Status: "In progress"),
        new(
            "distributed-dotnet-messaging",
            "Distributed .NET: gRPC, RabbitMQ & Gateways",
            "Design sync/async boundaries, YARP gateways, and service orchestration patterns used in banking platforms.",
            "Intermediate",
            18,
            IsFree: true,
            Topics: ["gRPC", "RabbitMQ", "YARP", "Docker"],
            Outcomes:
            [
                "Choose gRPC vs messaging for a given flow",
                "Wire a reverse-proxy gateway",
                "Ship a small multi-service demo with Docker"
            ],
            Status: "In progress"),
        new(
            "azure-cicd-for-dotnet",
            "Azure CI/CD for .NET Developers",
            "From GitHub Actions to Azure Static Web Apps / App Service — secrets, environments, and free-tier constraints for learning projects.",
            "Beginner",
            12,
            IsFree: true,
            Topics: ["GitHub Actions", "Azure SWA", "Secrets", "Blazor WASM"],
            Outcomes:
            [
                "Deploy Blazor WASM on the free SWA SKU",
                "Manage deployment tokens safely",
                "Add PR staging and production gates"
            ],
            Status: "In progress")
    ];

    public static BlogArticle? FindPost(string slug) =>
        Posts.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public static Course? FindCourse(string slug) =>
        Courses.FirstOrDefault(c => c.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<BlogArticle> FeaturedPosts() =>
        Posts.Where(p => p.Featured).OrderByDescending(p => p.Published);

    public static IEnumerable<BlogArticle> LatestPosts(int take = 6) =>
        Posts.OrderByDescending(p => p.Published).Take(take);
}
