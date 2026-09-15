using HotelChecklist.Domain.Common;

namespace HotelChecklist.Domain.Entities;

public sealed class Asset : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public Guid AreaId { get; set; }

    public bool Active { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public Area Area { get; set; } = null!;

    public ICollection<ChecklistInstance> ChecklistInstances { get; set; } = [];
}
