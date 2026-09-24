using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistTemplate : IAuditable
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public bool IsSnapshot { get; set; }

    public string Name { get; set; } = string.Empty;

    public TaskExecutionMode ExecutionMode { get; set; } = TaskExecutionMode.Scheduled;

    public UserRole CreatedByRole { get; set; }

    public string? Description { get; set; }

    public Guid AreaId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public Area Area { get; set; } = null!;

    public ICollection<ChecklistTask> Tasks { get; set; } = [];

    public ICollection<ChecklistInstance> Instances { get; set; } = [];

    public ICollection<TemplateAsset> TemplateAssets { get; set; } = [];

    public ICollection<TemplateSchedule> TemplateSchedules { get; set; } = [];
}
