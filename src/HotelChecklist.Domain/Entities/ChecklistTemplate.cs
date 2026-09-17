using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTemplate : IAuditable
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid AreaId { get; set; }

    public ChecklistRecurrenceType RecurrenceType { get; set; }

    public int EstimatedDurationMinutes { get; set; }

    public TimeOnly ScheduledTime { get; set; }

    public DateOnly RecurrenceStartDate { get; set; }

    public CustomRecurrenceMode? CustomRecurrenceMode { get; set; }

    public int? RecurrenceIntervalValue { get; set; }

    public RecurrenceIntervalUnit? RecurrenceIntervalUnit { get; set; }

    public int? RecurrenceDaysOfWeekMask { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public Area Area { get; set; } = null!;

    public ICollection<ChecklistTask> Tasks { get; set; } = [];

    public ICollection<ChecklistInstance> Instances { get; set; } = [];

    public ICollection<TemplateAsset> TemplateAssets { get; set; } = [];
}
