using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Entities;

public sealed class ChecklistInstance : IAuditable
{
    public Guid Id { get; set; }

    public Guid TemplateId { get; set; }

    public Guid AssetId { get; set; }

    public DateOnly Date { get; set; }

    public ChecklistStatus Status { get; set; } = ChecklistStatus.Pending;

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public long? DurationSeconds { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ChecklistTemplate Template { get; set; } = null!;

    public Asset Asset { get; set; } = null!;

    public ICollection<ChecklistTaskExecution> TaskExecutions { get; set; } = [];
}
