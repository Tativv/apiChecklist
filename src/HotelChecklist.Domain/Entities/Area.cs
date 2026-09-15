using HotelChecklist.Domain.Common;

namespace HotelChecklist.Domain.Entities;

public sealed class Area : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ICollection<Asset> Assets { get; set; } = [];

    public ICollection<ChecklistTemplate> ChecklistTemplates { get; set; } = [];
}
