using Friday.BuildingBlocks.Domain.Entities;
using Friday.Modules.Salon.Domain.Enums;

namespace Friday.Modules.Salon.Domain.Entities;

public sealed class Appointment : Entity
{
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int HairServiceId { get; set; }
    public HairService? HairService { get; set; }
    public int? StylistId { get; set; }
    public Stylist? Stylist { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Notes { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
}
