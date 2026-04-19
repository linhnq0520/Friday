using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Admin.Domain.Aggregates.RightAggregate;

/// <summary>
/// A permission row: bounded context (module) + resource + access tier. Keys are stable and catalog-driven.
/// </summary>
public sealed class Right : AggregateRoot
{
    private Right() { }

    public string Module { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public PermissionAccessLevel AccessLevel { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    /// <summary>Stable string for APIs, logs, and optional JWT claims (e.g. <c>admin:users:manage</c>).</summary>
    public string PermissionKey => PermissionKeys.Format(Module, Resource, AccessLevel);

    public static Right Create(
        string module,
        string resource,
        PermissionAccessLevel accessLevel,
        string name,
        string? description = null
    )
    {
        if (string.IsNullOrWhiteSpace(module))
        {
            throw new ArgumentException("Module is required.", nameof(module));
        }

        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Resource is required.", nameof(resource));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        return new Right
        {
            Module = NormalizeModule(module),
            Resource = NormalizeResource(resource),
            AccessLevel = accessLevel,
            Name = name.Trim(),
            Description = description?.Trim() ?? string.Empty,
        };
    }

    private static string NormalizeModule(string module) => module.Trim().ToLowerInvariant();

    private static string NormalizeResource(string resource) => resource.Trim().ToLowerInvariant();
}
