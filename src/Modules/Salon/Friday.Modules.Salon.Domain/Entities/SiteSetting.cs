using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class SiteSetting : Entity
{
    public string SettingKey { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
